using System.Collections.Generic;
using System.Threading.Tasks;

namespace Triggr.Services
{
    public interface IContainerService
    {
         Task<IEnumerable<Container>> CheckAsync();
    }
}