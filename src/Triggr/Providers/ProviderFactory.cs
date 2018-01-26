using System.Collections.Generic;
using System.Linq;
using Triggr.Services;

namespace Triggr.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IStorage _storage;
        private readonly IEnumerable<IProvider> _providers;
        public ProviderFactory(IEnumerable<IProvider> providers)
        {
            _providers = providers;
        }
        public IProvider GetProvider(string type)
        {
            var provider = _providers.FirstOrDefault(i => i.GetProviderType.Equals(type));
            return provider;
        }

        public string GetProviderType(string url)
        {
            var provider = _providers.FirstOrDefault(i => i.IsValid(url));
            return provider != null ? provider.GetProviderType : string.Empty;
        }
    }
}