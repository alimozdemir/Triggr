using Microsoft.Extensions.Options;
using Moq;
using Triggr.Services;
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

            WebhookService service = new WebhookService(null, mock.Object);

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

            WebhookService service = new WebhookService(null, mock.Object);

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

            WebhookService service = new WebhookService(null, mock.Object);

            Assert.False(service.IsSupport("http://bitbucket.com/lyzerk/TriggrTestProject"));
        }

        [Fact]
        public void AddHookInvalid()
        {
            var config = new TriggrConfig();
            config.Url = "http://www.triggr.com/";
            config.Webhook = true;

            var mock = new Mock<IOptions<TriggrConfig>>();
            mock.Setup(i => i.Value).Returns(config);

            WebhookService service = new WebhookService(null, mock.Object);
            var repo = new Data.Repository();
            repo.Token = "1";
            repo.Url = "http://github.com/lyzerk/TriggrTestProject";
            //service.AddHook(repo);
            //Assert.False(service.IsSupport(repo));
        }
        
    }
}