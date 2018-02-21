using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Microsoft.EntityFrameworkCore;
using Triggr.Data;
using Triggr.Providers;
using System;
using Triggr.Infrastructure;

namespace Triggr.Services
{
    public class ContainerService : IContainerService
    {
        private readonly TriggrContext _context;
        private readonly IProviderFactory _providerFactory;
        private readonly RepositoryStorage _storage;
        public ContainerService(TriggrContext context, IProviderFactory providerFactory, RepositoryStorage storage)
        {
            _context = context;
            _providerFactory = providerFactory;
            _storage = storage;
        }

        public async Task<IEnumerable<Container>> CheckAsync()
        {
            var repositories = await _context.Repositories.ToListAsync();

            List<Container> containers = new List<Container>();

            foreach (var repository in repositories)
            {
                var provider = _providerFactory.GetProvider(repository.Provider);
                string path = string.Empty;

                if (!provider.Exist(repository))
                {
                    path = provider.Clone(repository);
                }
                else
                    path = _storage.Combine(repository.Id);

                var container = new Container($"Container #{repository.Id}", path, repository)
                {
                    UpdatedTime = repository.UpdatedTime
                };

                containers.Add(container);
            }


            return containers;
        }

        public Container GetContainer(string repositoryId)
        {
            Container result = null;

            var repository = _context.Repositories.Find(repositoryId);

            if (repository != null)
            {
                result = GetContainer(repository);
            }

            return result;
        }

        public Container GetContainer(Repository repository)
        {
            Container result = null;

            var provider = _providerFactory.GetProvider(repository.Provider);
            string path = string.Empty;

            if (!provider.Exist(repository))
            {
                path = provider.Clone(repository);
            }
            else
                path = _storage.Combine(repository.Id);

            result = new Container($"Container #{repository.Id}", path, repository)
            {
                UpdatedTime = repository.UpdatedTime
            };

            return result;
        }
    }
}