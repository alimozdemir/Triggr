using System;
using System.Collections.Generic;
using Moq;
using Triggr.Providers;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{
    public class ProviderFactoryTest
    {
        [Fact]
        public void ConstructorWithNoList()
        {
            Action act = () => new ProviderFactory(null);
            Assert.ThrowsAny<ArgumentNullException>(act);
        }

        [Fact]
        public void GetProviderWithValidType()
        {
            var mockProvider = new Mock<GitProvider>();
            var list = new List<IProvider>(){
                mockProvider.Object
            };

            var factory = new ProviderFactory(list);

            var provider = factory.GetProvider("Git");

            Assert.Equal(mockProvider.Object, provider);
        }

        [Fact]
        public void GetProviderWithInvalidType()
        {
            var mockProvider = new Mock<GitProvider>();
            var list = new List<IProvider>(){
                mockProvider.Object
            };

            var factory = new ProviderFactory(list);

            Action service = () => factory.GetProvider("Test");
            Assert.ThrowsAny<ArgumentNullException>(service);
        }


        [Fact]
        public void GetProviderTypeWithValidType()
        {
            var mockProvider = new Mock<GitProvider>();
            mockProvider.Setup(i => i.IsValid("http://github.com/test")).Returns(true);

            var list = new List<IProvider>(){
                mockProvider.Object
            };

            var factory = new ProviderFactory(list);

            var type = factory.GetProviderType("http://github.com/test");

            Assert.Equal(mockProvider.Object.GetProviderType, type);
        }

        [Fact]
        public void GetProviderTypeWithInvalidType()
        {
            var mockProvider = new Mock<GitProvider>();
            mockProvider.Setup(i => i.IsValid("http://github.com/test")).Returns(false);

            var list = new List<IProvider>(){
                mockProvider.Object
            };

            var factory = new ProviderFactory(list);

            var type = factory.GetProviderType("http://github.com/test");

            Assert.Equal(string.Empty, type);
        }
    }
}