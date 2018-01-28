using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire.Console;
using Hangfire.Server;
using Triggr.Providers;
using Triggr.Services;

namespace Triggr
{
    public class ProbeControl
    {
        private readonly IProviderFactory _providerFactory;
        private readonly ILanguageService _languageService;
        private readonly IScriptExecutor _scriptExecutor;
        public ProbeControl(IProviderFactory providerFactory, ILanguageService languageService, IScriptExecutor scriptExecutor)
        {
            _providerFactory = providerFactory;
            _languageService = languageService;
            _scriptExecutor = scriptExecutor;
        }

        public void Execute(PerformContext hangfireContext, Container container)
        {
            if(!container.IsTriggrProject())
                return;
            
            var probes = container.CheckForProbes();
            hangfireContext.WriteLine($"Executing ProbeControl");

            var codeChangeProbes = probes.Where(i => i.ProbeType == ProbeType.CodeChanges).ToList();

            var oldFiles = new Dictionary<Probe, string>();
            if (codeChangeProbes.Count > 0)
            {
                foreach (var probe in codeChangeProbes)
                {
                    var tempFile = Path.GetTempFileName();

                    var objectPath = Path.Combine(container.Folder, probe.ObjectPath);
                    if (File.Exists(objectPath))
                    {
                        hangfireContext.WriteLine($"Probe {probe.ObjectPath}");

                        File.Copy(objectPath, tempFile, true);

                        oldFiles.Add(probe, tempFile);
                    }
                }
            }

            var provider = _providerFactory.GetProvider(container.Repository.Provider);
            var updatedPath = provider.Update(container.Repository);

            if (!string.IsNullOrEmpty(updatedPath))
            {
                foreach (var probe in probes)
                {
                    if (probe.ProbeType == ProbeType.CodeChanges)
                    {
                        var tempFile = oldFiles[probe];
                        var objectPath = Path.Combine(container.Folder, probe.ObjectPath);
                        var language = _languageService.Define(probe.ObjectPath);

                        var result = _scriptExecutor.Execute(probe.ProbeType, language, 
                                        objectPath, tempFile, probe.ObjectType, probe.ObjectName);
                                        
                        hangfireContext.WriteLine($"Result of comparision {result}");
                        // for now
                        /*var tempData = File.ReadAllText(tempFile);
                        var objectData = File.ReadAllText(objectPath);

                        if (tempData.Equals(objectData))
                        {
                            hangfireContext.WriteLine($"File does not changed {probe.ObjectPath}");
                        }
                        else
                        {
                            hangfireContext.WriteLine($"File is changed {probe.ObjectPath}");
                        }*/
                        // it will changed with executing shell script
                    }
                }
            }
            hangfireContext.WriteLine($"Finished ProbeControl");

        }
    }
}