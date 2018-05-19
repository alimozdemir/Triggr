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
        private readonly LibGit2SharpWrapper _git;
        private readonly RepositoryStorage _storage;
        private readonly IScriptExecutor _scriptExecutor;

        public GitProvider()
        {

        }
        public GitProvider(RepositoryStorage storage, IScriptExecutor scriptExecutor, LibGit2SharpWrapper git)
        {
            _git = git;
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

            var cloneResult = _git.Clone(data.Url, path);
            // hotfix: it was returning a path with .git (e.g. 'repositories/1/.git')
            // but we need absolute path of the repository, so I changed it to path from cloneResult

            return string.IsNullOrEmpty(cloneResult) ? "" : path;
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
                return _git.IsValid(path);
        }

        public virtual bool IsValid(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            return Regex.IsMatch(url, @"((git|ssh|http(s)?)|(git@[\w\.]+))(:(//)?)([\w\.@\:/\-~]+)(\.git)(/)?");
        }


        public string Update(Data.Repository data)
        {
            bool result = false;

            var path = _storage.Combine(data.Id);

            result = _git.Update(path);

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
    }
}