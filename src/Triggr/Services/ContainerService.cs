using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Microsoft.EntityFrameworkCore;
using Triggr.Data;
using Triggr.Providers;
using System;

namespace Triggr.Services
{
    public class ContainerService : IContainerService
    {
        private readonly TriggrContext _context;
        private readonly IProviderFactory _providerFactory;
        public ContainerService(TriggrContext context, IProviderFactory providerFactory)
        {
            _context = context;
            _providerFactory = providerFactory;
        }

        public async Task<IEnumerable<Container>> CheckAsync()
        {
            var repositories = await _context.Repositories.ToListAsync();
            List<Container> containers = new List<Container>();
            bool anyUpdate = false;

            foreach (var repository in repositories)
            {
                var provider = _providerFactory.GetProvider(repository.Provider);
                var updatedPath = provider.Update(repository);
                if (!string.IsNullOrEmpty(updatedPath))
                {
                    repository.UpdatedTime = DateTimeOffset.Now;

                    containers.Add(new Container($"Container #{repository.Id}", updatedPath)
                    {
                        UpdatedTime = repository.UpdatedTime
                    });

                    anyUpdate = true;
                }
            }

            if (anyUpdate)
            {
                await _context.SaveChangesAsync();
            }

            return containers;
        }
    }
}