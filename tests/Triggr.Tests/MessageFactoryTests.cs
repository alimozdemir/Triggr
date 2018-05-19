using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Moq;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{
    public class MessageFactoryTests
    {
        [Fact]
        public void ConstructorWithNoList()
        {
            Action act = () => new MessageFactory(null);
            Assert.ThrowsAny<ArgumentNullException>(act);
        }

        [Fact]
        public void GetProviderWithValidType()
        {
            var mockConfig = new Mock<IOptions<EmailConfig>>();
            var emailService = new EmailService(mockConfig.Object);
            var list = new List<IMessageService>(){
                emailService
            };

            var factory = new MessageFactory(list);

            var service = factory.GetMessageService(ActuatorType.Email);

            Assert.Equal(emailService, service);
            Assert.Equal(emailService.MessageType, service.MessageType);
        }

        [Fact]
        public void GetProviderWithInvalidType()
        {
            var mockConfig = new Mock<IOptions<EmailConfig>>();
            var emailService = new EmailService(mockConfig.Object);
            var list = new List<IMessageService>(){
                emailService
            };

            var factory = new MessageFactory(list);

            Action service = () => factory.GetMessageService(ActuatorType.GitHubIssue);
            Assert.ThrowsAny<ArgumentNullException>(service);
        }

    }
}