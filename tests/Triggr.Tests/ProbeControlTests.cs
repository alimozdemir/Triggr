using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using Triggr.Data;
using Triggr.Infrastructure;
using Triggr.Providers;
using Triggr.Services;
using Triggr.Tests.Infrastructures;
using Xunit;

namespace Triggr.Tests
{
    public class ProbeControlTests
    {
        private (Mock<TriggrContext> mockContext, Mock<DbSet<T>> mockSet) GetDatabaseMocks<T>(IQueryable<T> data) where T : class
        {
            var context = new Mock<TriggrContext>();
            var set = new Mock<DbSet<T>>();
            set.As<IAsyncEnumerable<T>>()
                .Setup(d => d.GetEnumerator())
                .Returns(new AsyncEnumerator<T>(data.GetEnumerator()));

            set.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            set.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            set.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            set.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            return (context, set);
        }

        [Fact]
        public void CodeChangesDetectionWithOneActuator()
        {
            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };

            var language = new LanguageProperties()
            {
                FolderName = "JavaScript",
                FullProject = false
            };
            var act1 = new Actuator() { Type = ActuatorType.Email, Emails = new List<string>() };
            var actuators = new List<Actuator>()
            {
                act1
            };

            var probe = new Probe()
            {
                Id = "1",
                ProbeType = ProbeType.CodeChanges,
                Object = new ObjectInformation() { Path = "test.js" },
                Actuators = actuators
            };

            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();

            var mockDb = GetDatabaseMocks(data);
            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);
            mockDb.mockSet.Setup(i => i.Find("1")).Returns(repo);

            var probes = new List<Probe>(){
                probe
            };

            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockScriptExecutor = new Mock<IScriptExecutor>();
            var mockContainerService = new Mock<IContainerService>();
            var mockContainer = new Mock<Container>();
            var mockProvider = new Mock<IProvider>();
            var mockMessageFactory = new Mock<IMessageFactory>();
            var mockMessageService = new Mock<IMessageService>();


            mockContainer.Setup(i => i.CheckForProbes())
                         .Returns(probes);

            mockContainer.SetupGet(i => i.Folder)
                         .Returns("Test");

            mockContainerService.Setup(i => i.GetContainer("1"))
                                .Returns(mockContainer.Object);

            mockLanguageService.Setup(i => i.Define("test.js"))
                               .Returns(language);

            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider))
                                .Returns(mockProvider.Object);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, true))
                                .Returns(true);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, false))
                                .Returns(true);

            var objectPath = Path.Combine(mockContainer.Object.Folder, probe.Object.Path);

            mockScriptExecutor.SetupSequence(i => i.Execute("AST", language.FolderName, objectPath, probe.Object.Type, probe.Object.Name))
                              .Returns("ast1")
                              .Returns("ast2");

            mockMessageFactory.Setup(i => i.GetMessageService(act1.Type))
                              .Returns(mockMessageService.Object);


            var control = new ProbeControl(mockProviderFactory.Object,
                                            mockLanguageService.Object,
                                            mockScriptExecutor.Object,
                                            mockDb.mockContext.Object,
                                            mockContainerService.Object,
                                            mockMessageFactory.Object);

            control.Execute(null, "1", "1");

            mockMessageService.Verify(i => i.Send(It.IsAny<Data.Repository>(), act1, $"{probe.Object.Name} is changed."));


        }

        [Fact]
        public void CodeChangesDetectionWithTwoActuators()
        {
            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };

            var language = new LanguageProperties()
            {
                FolderName = "JavaScript",
                FullProject = false
            };
            var act1 = new Actuator() { Type = ActuatorType.Email, Emails = new List<string>() };
            var act2 = new Actuator() { Type = ActuatorType.GitHubIssue };
            var actuators = new List<Actuator>()
            {
                act1, act2
            };

            var probe = new Probe()
            {
                Id = "1",
                ProbeType = ProbeType.CodeChanges,
                Object = new ObjectInformation() { Path = "test.js" },
                Actuators = actuators
            };

            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();

            var mockDb = GetDatabaseMocks(data);
            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);
            mockDb.mockSet.Setup(i => i.Find("1")).Returns(repo);

            var probes = new List<Probe>(){
                probe
            };

            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockScriptExecutor = new Mock<IScriptExecutor>();
            var mockContainerService = new Mock<IContainerService>();
            var mockContainer = new Mock<Container>();
            var mockProvider = new Mock<IProvider>();
            var mockMessageFactory = new Mock<IMessageFactory>();
            var mockMessageService = new Mock<IMessageService>();


            mockContainer.Setup(i => i.CheckForProbes())
                         .Returns(probes);

            mockContainer.SetupGet(i => i.Folder)
                         .Returns("Test");

            mockContainerService.Setup(i => i.GetContainer("1"))
                                .Returns(mockContainer.Object);

            mockLanguageService.Setup(i => i.Define("test.js"))
                               .Returns(language);

            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider))
                                .Returns(mockProvider.Object);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, true))
                                .Returns(true);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, false))
                                .Returns(true);

            var objectPath = Path.Combine(mockContainer.Object.Folder, probe.Object.Path);

            mockScriptExecutor.SetupSequence(i => i.Execute("AST", language.FolderName, objectPath, probe.Object.Type, probe.Object.Name))
                              .Returns("ast1")
                              .Returns("ast2");

            mockMessageFactory.Setup(i => i.GetMessageService(act1.Type))
                              .Returns(mockMessageService.Object);

            mockMessageFactory.Setup(i => i.GetMessageService(act2.Type))
                              .Returns(mockMessageService.Object);

            var control = new ProbeControl(mockProviderFactory.Object,
                                            mockLanguageService.Object,
                                            mockScriptExecutor.Object,
                                            mockDb.mockContext.Object,
                                            mockContainerService.Object,
                                            mockMessageFactory.Object);

            control.Execute(null, "1", "1");

            mockMessageService.Verify(i => i.Send(It.IsAny<Data.Repository>(), It.IsAny<Actuator>(), $"{probe.Object.Name} is changed."), Times.Exactly(2));


        }

        [Fact]
        public void CodeChangesNoDetectionWithOneActuator()
        {
            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };

            var language = new LanguageProperties()
            {
                FolderName = "JavaScript",
                FullProject = false
            };
            var act1 = new Actuator() { Type = ActuatorType.Email };
            var actuators = new List<Actuator>()
            {
                act1
            };

            var probe = new Probe()
            {
                Id = "1",
                ProbeType = ProbeType.CodeChanges,
                Object = new ObjectInformation() { Path = "test.js" },
                Actuators = actuators
            };

            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();

            var mockDb = GetDatabaseMocks(data);
            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);
            mockDb.mockSet.Setup(i => i.Find("1")).Returns(repo);

            var probes = new List<Probe>(){
                probe
            };

            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockScriptExecutor = new Mock<IScriptExecutor>();
            var mockContainerService = new Mock<IContainerService>();
            var mockContainer = new Mock<Container>();
            var mockProvider = new Mock<IProvider>();
            var mockMessageFactory = new Mock<IMessageFactory>();
            var mockMessageService = new Mock<IMessageService>();


            mockContainer.Setup(i => i.CheckForProbes())
                         .Returns(probes);

            mockContainer.SetupGet(i => i.Folder)
                         .Returns("Test");

            mockContainerService.Setup(i => i.GetContainer("1"))
                                .Returns(mockContainer.Object);

            mockLanguageService.Setup(i => i.Define("test.js"))
                               .Returns(language);

            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider))
                                .Returns(mockProvider.Object);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, true))
                                .Returns(true);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, false))
                                .Returns(true);

            var objectPath = Path.Combine(mockContainer.Object.Folder, probe.Object.Path);

            mockScriptExecutor.SetupSequence(i => i.Execute("AST", language.FolderName, objectPath, probe.Object.Type, probe.Object.Name))
                              .Returns("ast1")
                              .Returns("ast1");

            mockMessageFactory.Setup(i => i.GetMessageService(act1.Type))
                              .Returns(mockMessageService.Object);


            var control = new ProbeControl(mockProviderFactory.Object,
                                            mockLanguageService.Object,
                                            mockScriptExecutor.Object,
                                            mockDb.mockContext.Object,
                                            mockContainerService.Object,
                                            mockMessageFactory.Object);

            control.Execute(null, "1", "1");

            mockMessageService.Verify(i => i.Send(It.IsAny<Data.Repository>(), It.IsAny<Actuator>(), It.IsAny<string>()), Times.Never());
        }

        /*[Fact]
        public void StaticAnalysisAlwaysWithOneActuator()
        {
            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };

            var language = new LanguageProperties()
            {
                FolderName = "JavaScript",
                FullProject = false
            };
            var act1 = new Actuator() { Type = ActuatorType.Email };
            var actuators = new List<Actuator>()
            {
                act1
            };

            var probe = new Probe()
            {
                Id = "1",
                ProbeType = ProbeType.StaticAnalysis,
                Object = new ObjectInformation() { Path = "test.js" },
                Actuators = actuators,
                Metrics = new Metrics()
                {
                    Strategy = ReportType.Always,
                    Arguments = new List<string>() { "arg1" }
                }
            };

            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();

            var mockDb = GetDatabaseMocks(data);
            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);
            mockDb.mockSet.Setup(i => i.Find("1")).Returns(repo);

            var probes = new List<Probe>(){
                probe
            };

            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockScriptExecutor = new Mock<IScriptExecutor>();
            var mockContainerService = new Mock<IContainerService>();
            var mockContainer = new Mock<Container>();
            var mockProvider = new Mock<IProvider>();
            var mockMessageFactory = new Mock<IMessageFactory>();
            var mockMessageService = new Mock<IMessageService>();


            mockContainer.Setup(i => i.CheckForProbes())
                         .Returns(probes);

            mockContainer.SetupGet(i => i.Folder)
                         .Returns("Test");

            mockContainerService.Setup(i => i.GetContainer("1"))
                                .Returns(mockContainer.Object);

            mockLanguageService.Setup(i => i.Define("test.js"))
                               .Returns(language);

            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider))
                                .Returns(mockProvider.Object);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, true))
                                .Returns(true);

            mockProvider.Setup(i => i.Restore(repo, probe.Object.Path, false))
                                .Returns(true);

            var objectPath = Path.Combine(mockContainer.Object.Folder, probe.Object.Path);
            var parameters = new List<string>();
            
            parameters.Add(It.IsAny<string>());
            parameters.AddRange(probe.Metrics.Arguments.Select(i => $"'{i}'"));

            mockScriptExecutor.SetupSequence(i => i.Execute(probe.ProbeType, language.FolderName, parameters.ToArray()))
                              .Returns("result1")
                              .Returns("result2");

            mockMessageFactory.Setup(i => i.GetMessageService(act1.Type))
                              .Returns(mockMessageService.Object);


            var control = new ProbeControl(mockProviderFactory.Object,
                                            mockLanguageService.Object,
                                            mockScriptExecutor.Object,
                                            mockDb.mockContext.Object,
                                            mockContainerService.Object,
                                            mockMessageFactory.Object);

            control.Execute(null, "1", "1");

            mockMessageService.Verify(i => i.Send(act1, $"{probe.Object.Name} static analysis results. result2"));


        }*/
    }
}