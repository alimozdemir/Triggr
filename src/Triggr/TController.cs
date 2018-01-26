using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Triggr.Services;
using System.Linq;
using System;
using Hangfire;

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
                    hangfireContext.WriteLine("Starting to check.");
                    var containers = await _containerService.CheckAsync();

                    foreach (var container in containers)
                    {
                        hangfireContext.WriteLine(container.Name + " " + container.Folder);
                        BackgroundJob.Enqueue<ProbeControl>(i => i.Execute(null, container));
                        hangfireContext.WriteLine("ProbeControl Job is enqueued");
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