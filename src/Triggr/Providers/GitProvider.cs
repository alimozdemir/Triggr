using System;
using System.Threading.Tasks;
using System.Linq;
using Triggr.Services;
using System.Collections.Generic;
using LibGit2Sharp;
using System.IO;
using System.Text.RegularExpressions;
using Triggr.Infrastructure;

namespace Triggr.Providers
{
    public class GitProvider : IProvider
    {
        private readonly RepositoryStorage _storage;
        private readonly IScriptExecutor _scriptExecutor;

        public GitProvider(RepositoryStorage storage, IScriptExecutor scriptExecutor)
        {
            _storage = storage;
            _scriptExecutor = scriptExecutor;
        }

        public string GetProviderType => "Git";

        public string LastUpdatedFiles(Data.Repository data)
        {
            string result = string.Empty;
            var path = _storage.Combine(data.Id);

            result = _scriptExecutor.ExecuteCommon("LastUpdatedFiles", path);

            return result;
        }
        public string Clone(Data.Repository data)
        {
            var path = _storage.Combine(data.Id);

            var cloneResult = Repository.Clone(data.Url, path);

            return cloneResult;
        }

        public bool Exist(Data.Repository data)
        {
            var path = _storage.Combine(data.Id);

            // ensure the directory is created
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return false;
            }
            else
                return Repository.IsValid(path);
        }

        public bool IsValid(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return Regex.IsMatch(url, @"((git|ssh|http(s)?)|(git@[\w\.]+))(:(//)?)([\w\.@\:/\-~]+)(\.git)(/)?");
        }


        public string Update(Data.Repository data)
        {
            bool result = false;

            var path = _storage.Combine(data.Id);

            using (var repo = new Repository(path))
            {
                LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                options.FetchOptions = new FetchOptions();

                Signature author = new Signature("triggr", "triggr@itu.edu.tr", DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, options);

                //conflict check requires
                result = mergeResult.Status != MergeStatus.UpToDate;
            }

            return result ? path : string.Empty;
        }

        public bool Restore(Data.Repository data, string fileName, bool previousFile)
        {
            bool result = false;
            // HEAD = last commit, HEAD^ = previous commit of last commit
            var head = previousFile ? "HEAD^" : "HEAD";

            var path = _storage.Combine(data.Id);

            //script returns error messages or empty string.
            var commandResult = _scriptExecutor.ExecuteCommon("Restore", path, head, fileName);

            // if it is empty, no problem with restore process
            result = string.IsNullOrEmpty(commandResult);

            return result;
        }


        /*
        private (string User, string Repository) Parse(string url)
        {
            var uri = new Uri(url);

            var paths = uri.AbsolutePath.Split('/');

            return (paths[0], paths[1]);
        } */
    }
}