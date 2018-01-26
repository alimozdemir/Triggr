using System;
using System.Threading.Tasks;
using System.Linq;
using Triggr.Services;
using System.Collections.Generic;
using LibGit2Sharp;
using System.IO;
using System.Text.RegularExpressions;

namespace Triggr.Providers
{
    public class GitProvider : IProvider
    {
        private readonly IStorage _storage;
        public GitProvider(IStorage storage)
        {
            _storage = storage;
        }

        public string GetProviderType => "Git";

        public bool IsValid(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return Regex.IsMatch(url, @"((git|ssh|http(s)?)|(git@[\w\.]+))(:(//)?)([\w\.@\:/\-~]+)(\.git)(/)?");
        }

        public string Update(Data.Repository data)
        {
            bool result = false;

            var path = _storage.Combine(data.Id.ToString());

            // ensure the directory is created
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);


            var isValid = Repository.IsValid(path);

            if (!isValid)
            {
                var cloneResult = Repository.Clone(data.Url, path);
                result = !string.IsNullOrEmpty(cloneResult);
            }
            else
            {
                var repo = new Repository(path);

                LibGit2Sharp.PullOptions options = new LibGit2Sharp.PullOptions();
                options.FetchOptions = new FetchOptions();
                
                Signature author = new Signature("triggr", "triggr@itu.edu.tr", DateTime.Now);
                var mergeResult = Commands.Pull(repo, author, options);

                //conflict check requires
                result = mergeResult.Status != MergeStatus.UpToDate;
            }

            return result ? path : string.Empty;
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