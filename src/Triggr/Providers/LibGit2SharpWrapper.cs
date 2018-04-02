using System;
using LibGit2Sharp;

namespace Triggr.Providers
{
    public class LibGit2SharpWrapper
    {
        public LibGit2SharpWrapper()
        {

        }

        public virtual string Clone(string url, string path) => Repository.Clone(url, path);
        public virtual bool IsValid(string path) => Repository.IsValid(path);
        public virtual bool Update(string path)
        {
            bool result = false;
            using (var repo = new Repository(path))
            {
                LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                options.FetchOptions = new FetchOptions();

                Signature author = new Signature("triggr", "triggr@itu.edu.tr", DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, options);

                //conflict check requires
                result = mergeResult.Status != MergeStatus.UpToDate;
            }
            return result;
        }
    }
}