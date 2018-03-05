using System.Threading.Tasks;

namespace Triggr.Services
{
    public interface IWebhookService
    {
         bool IsSupport(string url);
         Task<bool> AddHookAsync(Data.Repository repo);
         string WebhookUrl();
    }
}