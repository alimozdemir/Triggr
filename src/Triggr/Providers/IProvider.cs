using System.Threading.Tasks;
using Triggr.Data;

namespace Triggr.Providers
{
    public interface IProvider
    {
        string Update(Repository repository);
        bool Exist(string url);
    }
}