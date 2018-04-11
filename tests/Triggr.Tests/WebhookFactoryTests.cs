using System.Collections.Generic;
using Moq;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{
    public class WebhookFactoryTests
    {
        [Fact]
        public void IsSupportedTrueTest()
        {
            var mockGithub = new Mock<IWebhookService>();
            var mockBitbucket = new Mock<IWebhookService>();
            mockGithub.Setup(i => i.IsSupport("www.github.com")).Returns(true);
            mockBitbucket.Setup(i => i.IsSupport("www.github.com")).Returns(false);

            List<IWebhookService> services = new List<IWebhookService>() { mockGithub.Object, mockBitbucket.Object };
            WebhookFactory factory = new WebhookFactory(services);

            Assert.True(factory.IsSupported("www.github.com"));
        }

        [Fact]
        public void IsSupportedFalseTest()
        {
            var mockGithub = new Mock<IWebhookService>();
            var mockBitbucket = new Mock<IWebhookService>();
            mockGithub.Setup(i => i.IsSupport("www.githu1b.com")).Returns(false);
            mockBitbucket.Setup(i => i.IsSupport("www.githu1b.com")).Returns(false);

            List<IWebhookService> services = new List<IWebhookService>() { mockGithub.Object, mockBitbucket.Object };
            WebhookFactory factory = new WebhookFactory(services);

            Assert.False(factory.IsSupported("www.githu1b.com"));
        }

        [Fact]
        public void GetServiceWithGithubUrl()
        {
            var mockGithub = new Mock<IWebhookService>();
            var mockBitbucket = new Mock<IWebhookService>();
            mockGithub.Setup(i => i.IsSupport("www.github.com")).Returns(true);
            mockBitbucket.Setup(i => i.IsSupport("www.github.com")).Returns(false);

            List<IWebhookService> services = new List<IWebhookService>() { mockGithub.Object, mockBitbucket.Object };
            WebhookFactory factory = new WebhookFactory(services);

            Assert.Equal(mockGithub.Object, factory.GetService("www.github.com"));
        }

        [Fact]
        public void GetServiceWithBitbucketUrl()
        {
            var mockGithub = new Mock<IWebhookService>();
            var mockBitbucket = new Mock<IWebhookService>();
            mockGithub.Setup(i => i.IsSupport("www.bitbucket.com")).Returns(false);
            mockBitbucket.Setup(i => i.IsSupport("www.bitbucket.com")).Returns(true);

            List<IWebhookService> services = new List<IWebhookService>() { mockGithub.Object, mockBitbucket.Object };
            WebhookFactory factory = new WebhookFactory(services);

            Assert.Equal(mockBitbucket.Object, factory.GetService("www.bitbucket.com"));
        }

        [Fact]
        public void GetServiceWithUnknownUrl()
        {
            var mockGithub = new Mock<IWebhookService>();
            var mockBitbucket = new Mock<IWebhookService>();
            mockGithub.Setup(i => i.IsSupport("www.bitbucket.com")).Returns(false);
            mockBitbucket.Setup(i => i.IsSupport("www.bitbucket.com")).Returns(false);

            List<IWebhookService> services = new List<IWebhookService>() { mockGithub.Object, mockBitbucket.Object };
            WebhookFactory factory = new WebhookFactory(services);

            Assert.Null(factory.GetService("www.bitbucket.com"));
        }
    }
}