namespace Triggr.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private static string[] _listOfProviders = new string[] { "Github", "Bitbucket" };
        public IProvider GetProvider(string type)
        {
            return null;
        }

        public string GetProviderType(string url)
        {
            // url identification
            
            return "Github";
        }
    }
}