using System.Collections.Generic;
using System.Linq;
using Triggr.Services;
using System;

namespace Triggr.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IEnumerable<IProvider> _providers;
        public ProviderFactory(IEnumerable<IProvider> providers)
        {
            if (providers == null)
                throw new ArgumentNullException(nameof(providers));

            _providers = providers;
        }
        public IProvider GetProvider(string type)
        {
            var provider = _providers.FirstOrDefault(i => i.GetProviderType.Equals(type));

            if (provider == null)
                throw new ArgumentNullException(nameof(type));

            return provider;
        }

        public string GetProviderType(string url)
        {
            var provider = _providers.FirstOrDefault(i => i.IsValid(url));
            return provider != null ? provider.GetProviderType : string.Empty;
        }
    }
}