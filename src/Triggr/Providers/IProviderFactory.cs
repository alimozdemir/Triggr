using System;

namespace Triggr.Providers
{
    public interface IProviderFactory
    {
         IProvider GetProvider(string type);
         string GetProviderType(string url);
    }
}