using System.Collections.Generic;
using System.Linq;
using System;

namespace Triggr.Services
{
    public class MessageFactory : IMessageFactory
    {
        private readonly IEnumerable<IMessageService> _services;

        public MessageFactory(IEnumerable<IMessageService> services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IMessageService GetMessageService(ActuatorType type)
        {
            var service = _services.FirstOrDefault(i => i.MessageType.Equals(type));

            if (service == null)
                throw new ArgumentNullException(nameof(type));

            return service;
        }
    }
}