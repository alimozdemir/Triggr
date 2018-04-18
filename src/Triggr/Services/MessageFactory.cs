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
            _services = services;
        }

        public IMessageService GetMessageService(ActuatorType type)
        {
            var service = _services.FirstOrDefault(i => i.MessageType.Equals(type));

            if (service == null)
                throw new ArgumentNullException($"No service type registered for {type}");

            return service;
        }
    }
}