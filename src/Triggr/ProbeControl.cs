using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hangfire;
using Hangfire.Console;
using Hangfire.Server;
using Triggr.Data;
using Triggr.Infrastructure;
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
        private readonly IMessageFactory _messageFactory;

        public ProbeControl(IProviderFactory providerFactory, ILanguageService languageService,
                IScriptExecutor scriptExecutor,
                TriggrContext dbContext,
                IContainerService containerService,
                IMessageFactory messageFactory)
        {
            _providerFactory = providerFactory;
            _languageService = languageService;
            _scriptExecutor = scriptExecutor;
            _dbContext = dbContext;
            _containerService = containerService;
            _messageFactory = messageFactory;
        }

        /*public void Execute(PerformContext hangfireContext, Container container)
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

                        var result = _scriptExecutor.Execute(probe.ProbeType, language.FolderName,
                                        objectPath, tempFile, probe.Object.Type, probe.Object.Name);

                        hangfireContext.WriteLine($"Result of comparision {result}");

                        // it will changed with executing shell script
                    }
                }
            }
            hangfireContext.WriteLine($"Finished ProbeControl");

        }
        */
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

                    hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"{probe.Object.Path} is found.");
                    // define the language
                    var language = _languageService.Define(probe.Object.Path);
                    // get the provider
                    var provider = _providerFactory.GetProvider(repository.Provider);
                    // restore to previous file
                    if (provider.Restore(repository, probe.Object.Path, true))
                    {
                        hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"{probe.Object.Path} is restored to old.");
                        // get the file path
                        var objectPath = Path.Combine(container.Folder, probe.Object.Path);

                        // look for the object
                        var ast1 = _scriptExecutor.Execute("AST", language.FolderName, objectPath, probe.Object.Type, probe.Object.Name);

                        hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"{probe.Object.Path} old version is loaded.");

                        var temp1 = WriteToTemp(ast1);

                        // restore to original file
                        if (provider.Restore(repository, probe.Object.Path, false))
                        {
                            // look for the object
                            var ast2 = _scriptExecutor.Execute("AST", language.FolderName, objectPath, probe.Object.Type, probe.Object.Name);

                            var temp2 = WriteToTemp(ast2);

                            hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"{probe.Object.Path} new version is loaded.");

                            Control(hangfireContext, probe, repository, temp1, temp2, language.FolderName);
                        }
                    }
                }
            }
        }

        private void Control(PerformContext hangfireContext, Probe probe, Data.Repository repo, string temp1, string temp2, string language)
        {
            switch (probe.ProbeType)
            {
                case ProbeType.CodeChanges:

                    hangfireContext?.WriteLine(ConsoleTextColor.Green, "CodeChanges probe is activated.");
                    // load file1, and file2 for comparison
                    var file1 = File.ReadAllText(temp1);
                    var file2 = File.ReadAllText(temp2);

                    hangfireContext?.WriteLine(ConsoleTextColor.Gray, "Old version");
                    hangfireContext?.WriteLine(file1);

                    hangfireContext?.WriteLine(ConsoleTextColor.Gray, "New version");
                    hangfireContext?.WriteLine(file2);

                    if (!file1.Equals(file2))
                    {
                        hangfireContext?.WriteLine(ConsoleTextColor.Red, $"{probe.Object.Name} is changed.");
                        foreach (var act in probe.Actuators)
                        {
                            var service = _messageFactory.GetMessageService(act.Type);
                            service.Send(repo, act, $"{probe.Object.Name} is changed.");

                            ActuatorPrint(hangfireContext, act);
                        }

                    }
                    else
                        hangfireContext?.WriteLine(ConsoleTextColor.Red, $"{probe.Object.Name} isn't changed.");

                    break;
                case ProbeType.StaticAnalysis:
                    hangfireContext?.WriteLine(ConsoleTextColor.Green, "StaticAnalysis probe is activated.");
                    hangfireContext?.WriteLine($"Strategy of the probe is {probe.Metrics.Strategy}.");

                    // collect the parameters
                    List<string> parameters = new List<string>();

                    // for test
                    var tex = File.ReadAllText(temp2);
                    hangfireContext?.WriteLine(tex);

                    // add file path
                    parameters.Add(temp1);
                    // transform all parameters from a to 'a'
                    parameters.AddRange(probe.Metrics.Arguments.Select(i => $"'{i}'"));
                    // execute static analysis script
                    var result1 = _scriptExecutor.Execute(probe.ProbeType, language, parameters.ToArray());
                    // change the file path of parameters
                    parameters[0] = temp2;
                    // execute second static analysis script
                    var result2 = _scriptExecutor.Execute(probe.ProbeType, language, parameters.ToArray());

                    if (result2.Contains(temp2))
                        result2 = result2.Replace(temp2, probe.Object.Path);

                    if (result1.Contains(temp1))
                        result1 = result1.Replace(temp1, probe.Object.Path);

                    // report the results upon strategy
                    if (probe.Metrics.Strategy == ReportType.Always)
                    {
                        if (string.IsNullOrEmpty(result2) || result2.Equals("-1"))
                            hangfireContext?.WriteLine(ConsoleTextColor.Red, "No result.");
                        else
                        {


                            foreach (var act in probe.Actuators)
                            {
                                var service = _messageFactory.GetMessageService(act.Type);
                                service.Send(repo, act, $"{probe.Object.Name} static analysis results. {result2}");

                                ActuatorPrint(hangfireContext, act);
                            }
                            hangfireContext?.WriteLine(result2);
                        }

                    }
                    else if (probe.Metrics.Strategy == ReportType.Diff)
                    {
                        if (!result1.Equals(result2))
                        {
                            hangfireContext?.WriteLine("There is a difference between results");
                            hangfireContext?.WriteLine(result1);

                            foreach (var act in probe.Actuators)
                            {
                                var service = _messageFactory.GetMessageService(act.Type);
                                service.Send(repo, act, $"{probe.Object.Name} has different static analysis results between two commits." +
                                "Old" + Environment.NewLine +
                                      result1 + Environment.NewLine + "**************" + Environment.NewLine + result2);


                                ActuatorPrint(hangfireContext, act);
                            }
                        }
                    }

                    break;
            }

            // delete temporary files
            File.Delete(temp1);
            File.Delete(temp2);
        }

        private void ActuatorPrint(PerformContext hangfireContext, Actuator act)
        {
            hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"An actuator found, {act.Type}");
            if (act.Type == ActuatorType.Email)
            {
                foreach (var mail in act.Emails)
                    hangfireContext?.WriteLine(ConsoleTextColor.Gray, $"Email:{mail}");
            }
        }

        private string WriteToTemp(string data)
        {
            var temp = Path.GetTempFileName();

            File.WriteAllText(temp, data);

            return temp;
        }
    }
}