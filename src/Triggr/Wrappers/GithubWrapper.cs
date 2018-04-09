using System.Collections.Generic;
using System.Threading.Tasks;
using Octokit;

namespace Triggr.Wrappers
{
    public class GithubWrapper
    {
        private readonly GitHubClient _client;

        public GithubWrapper()
        {
            
        }
        public GithubWrapper(GitHubClient client)
        {
            _client = client;
        }

        public virtual Task<RepositoryHook> CreateWebhook(string ownerName, string repoName, string url, string token)
        {
            _client.Credentials = new Credentials(token);

            Dictionary<string, string> cfg = new Dictionary<string, string>();
            cfg.Add("url", url);
            cfg.Add("content_type", "json");

            var info = new NewRepositoryHook("web", cfg);
            return _client.Repository.Hooks.Create(ownerName, repoName, info);
        }
    }
}