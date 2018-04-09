using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Octokit;
using Triggr.Providers;
using Triggr.Wrappers;

namespace Triggr.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly IProviderFactory _providerFactory;
        private readonly TriggrConfig _config;
        private readonly GithubWrapper _client;

        public WebhookService(IProviderFactory providerFactory, IOptions<TriggrConfig> config, GithubWrapper client)
        {
            _providerFactory = providerFactory;
            _config = config.Value;
            _client = client;
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
            var createResult = await _client.CreateWebhook(repo.OwnerName, repo.Name, WebhookUrl(), repo.Token);
            
            repo.WebHook = createResult.Id != 0;
            repo.WebHookId = createResult.Id.ToString();

            return repo.WebHook;
        }
    }
}