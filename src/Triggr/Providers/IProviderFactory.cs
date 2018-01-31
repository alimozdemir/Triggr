using System;

namespace Triggr.Providers
{
    public interface IProviderFactory
    {
        /// <summary>
        /// Returns a provider with given a type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Provider (e.g. GitProvider)</returns>
         IProvider GetProvider(string type);
         /// <summary>
         /// Controls the given url with current providers
         /// </summary>
         /// <param name="url"></param>
         /// <returns>Empty if no match, if matched then return IProvider.GetProviderType</returns>
         string GetProviderType(string url);
    }
}