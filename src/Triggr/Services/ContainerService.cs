using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Triggr.Data;

namespace Triggr.Services
{
    public class ContainerService : IContainerService
    {
        private readonly TriggrContext _context;
        public ContainerService(TriggrContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Container>> CheckAsync()
        {
            var repositories = await _context.Repositories.ToListAsync();

            return Enumerable.Empty<Container>();
        }
    }
}