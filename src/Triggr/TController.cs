using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Triggr.Services;
using System.Linq;
using System;

namespace Triggr
{
    public class TController
    {
        private readonly IContainerService _containerService;
        public TController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        public void Tick(PerformContext hangfireContext)
        {
            Task.Run(async () =>
            {
                try
                {
                    var containers = await _containerService.CheckAsync();
                    hangfireContext.WriteLine("Starting to check.");

                    foreach (var container in containers)
                    {
                        hangfireContext.WriteLine(container.Name + " " + container.Folder);

                        var probes = container.CheckForProbes();

                        foreach (var probe in probes)
                        {
                            hangfireContext.WriteLine(probe.ProbeType + " " + probe.ObjectType + " " + probe.ObjectPath);
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