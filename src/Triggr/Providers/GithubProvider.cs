using System;
using System.Threading.Tasks;
using System.Linq;
using Octokit;
using Triggr.Data;
using Triggr.Services;
using System.Collections.Generic;

namespace Triggr.Providers
{
    public class GithubProvider : IProvider
    {
        private readonly IStorage _storage;
        private GitHubClient _client = new GitHubClient(new ProductHeaderValue("triggr"));
        private IEnumerable<GitHubCommit> _commits;
        public GithubProvider(IStorage storage)
        {
            _storage = storage;
            _commits = IEnumerable<GitHubCommit>.Empty();
        }
        public void Download()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> NewCommit()
        {
            bool result = false;
            var commits = await _client.Repository.Commit.GetAll("", "");
            //commits[0];
            //commits[0].Files[0].RawUrl
            _client.Repository.Commit.GetAll("", "", new ApiOptions() { });
            return result;
        }

        public async Task<bool> CheckForCommitsAsync(Data.Repository repository)
        {
            var github = Parse(repository.Url);

            var commits = await _client.Repository.Commit.GetAll(github.User, github.Repository);

            var newCommits = commits.TakeWhile(i => i.Ref.Equals(repository.Reference)).ToList();
            _commits = newCommits;
            return newCommits.Count > 0;
        }

        public async Task DownloadAsync(Data.Repository repository)
        {
            foreach (var commit in _commits)
            {
                foreach (var item in commit.Files)
                {
                    await _client.Connection.Get<string>(new Uri(item.RawUrl), TimeSpan.FromMinutes(1));

                }
            }
        }

        private (string User, string Repository) Parse(string url)
        {
            var uri = new Uri(url);

            var paths = uri.AbsolutePath.Split('/');

            return (paths[0], paths[1]);
        }
    }
}