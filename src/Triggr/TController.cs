using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Triggr.Services;
using System.Linq;
using System;
using Hangfire;
using Triggr.Providers;

namespace Triggr
{
    public class TController
    {
        private readonly IContainerService _containerService;
        private readonly IProviderFactory _providerFactory;

        public TController(IContainerService containerService, IProviderFactory providerFactory)
        {
            _containerService = containerService;
            _providerFactory = providerFactory;
        }

        public void Tick(PerformContext hangfireContext)
        {
            Task.Run(async () =>
            {
                try
                {
                    hangfireContext.WriteLine("Starting to check.");
                    var containers = await _containerService.CheckAsync();

                    foreach (var container in containers)
                    {
                        hangfireContext.WriteLine($"{container.Name} {container.Folder}");
                        var probes = container.CheckForProbes();

                        hangfireContext.WriteLine($"{container.Name} is updating..");

                        var provider = _providerFactory.GetProvider(container.Repository.Provider);

                        var fileList = container.Update(provider);

                        foreach (var item in fileList)
                        {
                            hangfireContext.WriteLine($"{container.Name} {item} file updated.");
                        }

                        hangfireContext.WriteLine($"{container.Name} is updated..");

                        var activatedProbes = probes.Where(i => fileList.Contains(i.Object.Path));

                        foreach (var item in activatedProbes)
                        {
                            hangfireContext.WriteLine($"{item.Object.Path} file's probe is activated.");
                            BackgroundJob.Enqueue<ProbeControl>(i => i.Execute(null, item.Id, container.Repository.Id));
                        }
                    }

                    hangfireContext.WriteLine($"Total found containers {containers.Count()}.");

                    hangfireContext.WriteLine("End of check.");
                }
                catch (Exception ex)
                {
                    hangfireContext.WriteLine(ConsoleTextColor.Red, ex);
                }


            });

        }
    }
}