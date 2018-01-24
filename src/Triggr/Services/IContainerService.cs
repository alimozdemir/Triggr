using System.Collections.Generic;
using System.Threading.Tasks;
using Hangfire.Server;

namespace Triggr.Services
{
    public interface IContainerService
    {
         Task<IEnumerable<Container>> CheckAsync();
    }
}