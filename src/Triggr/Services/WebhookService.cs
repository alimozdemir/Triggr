using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Octokit;
using Triggr.Providers;

namespace Triggr.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IProviderFactory _providerFactory;
        private readonly TriggrConfig _config;

        public WebhookService(IProviderFactory providerFactory, IOptions<TriggrConfig> config)
        {
            _providerFactory = providerFactory;
            _config = config.Value;
        }
        public async Task<bool> AddHookAsync(Data.Repository repo)
        {
            bool result = false;
            Uri uri = new Uri(repo.Url);

            if (uri.Host.Contains("github"))
            {
                result = await AddHookGithub(repo);
            }

            return result;
        }

        public bool IsSupport(string url)
        {
            bool result = false;
            Uri uri = new Uri(url);

            if (uri.Host.Contains("github"))
            {
                result = true;
            }

            return result;
        }

        public string WebhookUrl()
        {
            UriBuilder builder = new UriBuilder(_config.Url);
            builder.Path = "GithubWebhook/HandlerForPush";
            var url = builder.Uri.ToString();
            return url;
        }

        private async Task<bool> AddHookGithub(Data.Repository repo)
        {
            bool result = false;
            var github = new GitHubClient(new ProductHeaderValue("Triggr"));
            github.Credentials = new Credentials(repo.Token);

            Dictionary<string, string> cfg = new Dictionary<string, string>();
            cfg.Add("url", WebhookUrl());
            cfg.Add("content_type", "json");

            var info = new NewRepositoryHook("web", cfg);
            var createResult = await github.Repository.Hooks.Create(repo.OwnerName, repo.Name, info);
            repo.WebHook = createResult.Id != 0;
            repo.WebHookId = createResult.Id.ToString();

            return result;
        }
    }
}