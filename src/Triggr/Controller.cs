using System.Threading.Tasks;
using Hangfire.Server;
using Hangfire.Console;
using Triggr.Services;

namespace Triggr
{
    public class Controller
    {
        private readonly IContainerService _containerService;
        public Controller(IContainerService containerService)
        {
            _containerService = containerService;
        }

        public void Tick(PerformContext hangfireContext)
        {
            Task.Run(async () =>
            {
                var containers = await _containerService.CheckAsync();

                foreach (var container in containers)
                {
                    hangfireContext.WriteLine(container.Name + " " + container.Folder);

                    var probes = container.CheckForProbes();

                    foreach(var probe in probes)
                    {
                        hangfireContext.WriteLine(probe.ProbeType + " " + probe.ObjectType + " " + probe.ObjectPath);
                    }
                }
            });

        }
    }
}