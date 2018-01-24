using Triggr.Services;

namespace Triggr.Providers
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IStorage _storage;
        public ProviderFactory(IStorage storage)
        {
            _storage = storage;
        }
        private static string[] _listOfProviders = new string[] { "Github", "Bitbucket" };
        public IProvider GetProvider(string type)
        {
            switch (type)
            {
                case "Git":
                    return new GitProvider(_storage);
            }
            return null;
        }

        public string GetProviderType(string url)
        {
            // url identification

            return "Github";
        }
    }
}