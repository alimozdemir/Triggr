using Triggr.Providers;

namespace Triggr.Services
{
    public interface IWebhookFactory
    {
         IWebhookService GetService(string url);
         bool IsSupported(string url);
    }
}