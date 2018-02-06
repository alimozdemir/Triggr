using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire.Console;
using Hangfire.Server;
using Triggr.Data;
using Triggr.Providers;
using Triggr.Services;

namespace Triggr
{
    public class ProbeControl
    {
        private readonly IProviderFactory _providerFactory;
        private readonly ILanguageService _languageService;
        private readonly IScriptExecutor _scriptExecutor;
        private readonly TriggrContext _dbContext;
        private readonly IContainerService _containerService;

        public ProbeControl(IProviderFactory providerFactory, ILanguageService languageService,
                IScriptExecutor scriptExecutor,
                TriggrContext dbContext,
                IContainerService containerService)
        {
            _providerFactory = providerFactory;
            _languageService = languageService;
            _scriptExecutor = scriptExecutor;
            _dbContext = dbContext;
            _containerService = containerService;
        }

        public void Execute(PerformContext hangfireContext, Container container)
        {
            if (!container.IsTriggrProject())
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

                    var objectPath = Path.Combine(container.Folder, probe.Object.Path);
                    if (File.Exists(objectPath))
                    {
                        hangfireContext.WriteLine($"Probe {probe.Object.Path}");

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
                        var objectPath = Path.Combine(container.Folder, probe.Object.Path);
                        var language = _languageService.Define(probe.Object.Path);

                        var result = _scriptExecutor.Execute(probe.ProbeType, language,
                                        objectPath, tempFile, probe.Object.Type, probe.Object.Name);

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

        public void Execute(PerformContext hangfireContext, string probeId, string repoId)
        {
            var repository = _dbContext.Repositories.Find(repoId); // get the repository
            if (repository != null)
            {
                var container = _containerService.GetContainer(repoId); // get the container

                // get all probes and look for probeId
                var probes = container.CheckForProbes();
                var probe = probes.FirstOrDefault(i => i.Id.Equals(probeId));

                if (probe != null)
                {

                    hangfireContext.WriteLine($"{probe.Object.Path} is found.");
                    var language = _languageService.Define(probe.Object.Path); // define the language
                    var provider = _providerFactory.GetProvider(repository.Provider); // get the provider

                    if (provider.Restore(repository, probe.Object.Path, true)) // restore to previous file
                    {
                        hangfireContext.WriteLine($"{probe.Object.Path} restored to old.");

                        var objectPath = Path.Combine(container.Folder, probe.Object.Path); // get the file path

                        // look for the object
                        var ast1 = _scriptExecutor.Execute("AST", language, objectPath, probe.Object.Type, probe.Object.Name);

                        hangfireContext.WriteLine($"{probe.Object.Path} old version is loaded.");
                        var temp1 = WriteToTemp(ast1);

                        // restore to original file
                        if (provider.Restore(repository, probe.Object.Path, false))
                        {
                            // look for the object
                            var ast2 = _scriptExecutor.Execute("AST", language, objectPath, probe.Object.Type, probe.Object.Name);

                            var temp2 = WriteToTemp(ast2);

                            hangfireContext.WriteLine($"{probe.Object.Path} new version is loaded.");

                            Control(hangfireContext, probe, temp1, temp2);
                        }
                    }
                }
            }
        }

        private void Control(PerformContext hangfireContext, Probe probe, string temp1, string temp2)
        {
            switch (probe.ProbeType)
            {
                case ProbeType.CodeChanges:
                    var file1 = File.ReadAllText(temp1);
                    var file2 = File.ReadAllText(temp2);

                    if (!file1.Equals(file2))
                    {
                        hangfireContext.WriteLine($"{probe.Object.Path} is changed.");
                    }
                    else
                        hangfireContext.WriteLine($"{probe.Object.Path} no change.");

                    break;
                case ProbeType.StaticAnalysis:
                    break;
            }

            // delete temporary files
            File.Delete(temp1);
            File.Delete(temp2);
        }

        private string WriteToTemp(string data)
        {
            var temp = Path.GetTempFileName();

            File.WriteAllText(temp, data);

            return temp;
        }
    }
}