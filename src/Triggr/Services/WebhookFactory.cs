using System.Collections.Generic;
using Triggr.Providers;
using System.Linq;

namespace Triggr.Services
{
    public class WebhookFactory : IWebhookFactory
    {
        private readonly IEnumerable<IWebhookService> _services;

        public WebhookFactory(IEnumerable<IWebhookService> services)
        {
            _services = services;
        }
        public IWebhookService GetService(string url)
        {
            var service = _services.FirstOrDefault(i => i.IsSupport(url));

            return service;
        }

        public bool IsSupported(string url)
        {
            return _services.Any(i => i.IsSupport(url));
        }
    }
}