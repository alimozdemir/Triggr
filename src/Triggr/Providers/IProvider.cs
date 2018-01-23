using System.Threading.Tasks;
using Triggr.Data;

namespace Triggr.Providers
{
    public interface IProvider
    {
        Task<bool> CheckForCommitsAsync(Repository repository);
        Task DownloadAsync(Repository repository);
    }
}