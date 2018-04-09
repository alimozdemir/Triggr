using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Moq;
using Octokit;
using Triggr.Services;
using Triggr.Wrappers;
using Xunit;

namespace Triggr.Tests
{
    public class WebhookServiceTests
    {
        [Fact]
        public void IsSupportValidWithWWW()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            Assert.True(service.IsSupport("http://www.github.com/lyzerk/TriggrTestProject"));
        }

        [Fact]
        public void IsSupportValidWithoutWWW()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            Assert.True(service.IsSupport("http://github.com/lyzerk/TriggrTestProject"));
        }

        [Fact]
        public void IsSupportInvalid()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            Assert.False(service.IsSupport("http://bitbucket.com/lyzerk/TriggrTestProject"));
        }

        [Fact]
        public async Task AddHookInvalid()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);
            var repo = new Data.Repository();
            repo.Token = "1";
            repo.Url = "http://githu1b.com/lyzerk/TriggrTestProject";
            var result = await service.AddHookAsync(repo);
            Assert.False(result);
        }

        [Fact]
        public void WebhookUrlWithValid()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            var result = service.WebhookUrl();
            Assert.Equal(config.Url + "GithubWebhook/HandlerForPush", result);
        }
        [Fact]
        public void WebhookUrlWithLocalhost()
        {
            var config = new TriggrConfig();
            config.Url = "http://localhost/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            var result = service.WebhookUrl();
            Assert.Equal(config.Url + "GithubWebhook/HandlerForPush", result);
        }

        [Fact]
        public void WebhookUrlWithLocalhostAndNoProtocol()
        {
            var config = new TriggrConfig();
            config.Url = "localhost";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);

            var result = service.WebhookUrl();
            Assert.Equal($"http://{config.Url}/GithubWebhook/HandlerForPush", result);
        }


        [Fact]
        public void WebhookUrlWithEmptyString()
        {
            var config = new TriggrConfig();
            config.Url = string.Empty;
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object, null);
            Action action = () => service.WebhookUrl();
            Assert.ThrowsAny<UriFormatException>(action);
        }
        
        [Fact]
        public async Task AddHookValid()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var repo = new Data.Repository();
            repo.Token = "1";
            repo.Url = "http://github.com/lyzerk/TriggrTestProject";
            repo.OwnerName = "lyzerk";
            repo.Name = "TriggrTestProject";

            var mockResult = new RepositoryHook(1, null, null, null, DateTimeOffset.Now, DateTimeOffset.Now, null, null, false, null);
            var mockConfig = new Mock<IOptions<TriggrConfig>>();
            var mockClient = new Mock<GithubWrapper>();

            mockConfig.Setup(i => i.Value).Returns(config);
            mockClient.Setup(i => i.CreateWebhook(repo.OwnerName, repo.Name, config.Url + "GithubWebhook/HandlerForPush", "1"))
                        .ReturnsAsync(mockResult);

            WebhookService service = new WebhookService(null, mockConfig.Object, mockClient.Object);

            var result = await service.AddHookAsync(repo);
            Assert.True(result);
            Assert.True(repo.WebHook);
            Assert.NotNull(repo.WebHookId);
        }
    }
}