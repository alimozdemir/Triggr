using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Hangfire.Server;
using Microsoft.EntityFrameworkCore;
using Moq;
using Triggr.Data;
using Triggr.Providers;
using Triggr.Services;
using Triggr.Tests.Infrastructures;
using Xunit;
using Hangfire.Console;
using DataSuit;
using System;
using System.Linq.Expressions;
using Hangfire.Common;
using Hangfire.States;

namespace Triggr.Tests
{
    public class TControllerTests
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
        public void RegularTest()
        {
            var probes = new List<Probe>(){
                new Probe() { Id = "1", Object = new ObjectInformation() { Path = "test.js" } }
            };

            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };
            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();
            var mockContainer = new Mock<Container>();
            var mockContainerService = new Mock<IContainerService>();
            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var mockProvider = new Mock<IProvider>();

            var mockDb = GetDatabaseMocks(data);

            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);

            TController controller = new TController(mockContainerService.Object,
                                    mockProviderFactory.Object,
                                    mockDb.mockContext.Object,
                                    mockJobClient.Object);
            
            
            mockContainerService.Setup(i => i.GetContainer(repo)).Returns(mockContainer.Object);
            mockContainer.Setup(i => i.CheckForProbes()).Returns(probes);
            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(mockProvider.Object);
            mockContainer.Setup(i => i.Update(mockProvider.Object));
            var probe = probes.FirstOrDefault(i => i.Object.Path == "test.js");
            
            controller.Trigger(null, "Test1", "Test", new List<string>() { "test.js" });

            mockJobClient.Verify(x => x.Create(
                    It.Is<Job>(i => i.Method.Name == "Execute"),
                    It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public void TriggerWithNoRepository()
        {
            var probes = new List<Probe>(){
                new Probe() { Id = "1", Object = new ObjectInformation() { Path = "test.js" } }
            };

            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };
            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();
            var mockContainer = new Mock<Container>();
            var mockContainerService = new Mock<IContainerService>();
            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var mockProvider = new Mock<IProvider>();

            var mockDb = GetDatabaseMocks(data);

            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);

            TController controller = new TController(mockContainerService.Object,
                                    mockProviderFactory.Object,
                                    mockDb.mockContext.Object,
                                    mockJobClient.Object);
            
            
            mockContainerService.Setup(i => i.GetContainer(repo)).Returns(mockContainer.Object);
            mockContainer.Setup(i => i.CheckForProbes()).Returns(probes);
            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(mockProvider.Object);
            mockContainer.Setup(i => i.Update(mockProvider.Object));
            
            controller.Trigger(null, "Test2", "Test", new List<string>() { "test.js" });

            mockJobClient.Verify(x => x.Create(
                    It.Is<Job>(i => i.Method.Name == "Execute"),
                    It.IsAny<EnqueuedState>()), Times.Never());
        }

        [Fact]
        public void TriggerWithNoProbes()
        {
            var probes = new List<Probe>(){
                //new Probe() { Id = "1", Object = new ObjectInformation() { Path = "test.js" } }
            };

            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };
            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();
            var mockContainer = new Mock<Container>();
            var mockContainerService = new Mock<IContainerService>();
            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var mockProvider = new Mock<IProvider>();

            var mockDb = GetDatabaseMocks(data);

            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);

            TController controller = new TController(mockContainerService.Object,
                                    mockProviderFactory.Object,
                                    mockDb.mockContext.Object,
                                    mockJobClient.Object);
            
            
            mockContainerService.Setup(i => i.GetContainer(repo)).Returns(mockContainer.Object);
            mockContainer.Setup(i => i.CheckForProbes()).Returns(probes);
            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(mockProvider.Object);
            mockContainer.Setup(i => i.Update(mockProvider.Object));
            var probe = probes.FirstOrDefault(i => i.Object.Path == "test.js");
            
            controller.Trigger(null, "Test1", "Test", new List<string>() { "test.js" });

            mockJobClient.Verify(x => x.Create(
                    It.Is<Job>(i => i.Method.Name == "Execute"),
                    It.IsAny<EnqueuedState>()), Times.Never());
        }

        [Fact]
        public void TriggerWithMultipleProbes()
        {
            var probes = new List<Probe>(){
                new Probe() { Id = "1", Object = new ObjectInformation() { Path = "test.js" } },
                new Probe() { Id = "2", Object = new ObjectInformation() { Path = "sum.js" } }
            };

            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };
            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();
            var mockContainer = new Mock<Container>();
            var mockContainerService = new Mock<IContainerService>();
            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockJobClient = new Mock<IBackgroundJobClient>();
            var mockProvider = new Mock<IProvider>();

            var mockDb = GetDatabaseMocks(data);

            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);

            TController controller = new TController(mockContainerService.Object,
                                    mockProviderFactory.Object,
                                    mockDb.mockContext.Object,
                                    mockJobClient.Object);
            
            
            mockContainerService.Setup(i => i.GetContainer(repo)).Returns(mockContainer.Object);
            mockContainer.Setup(i => i.CheckForProbes()).Returns(probes);
            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(mockProvider.Object);
            mockContainer.Setup(i => i.Update(mockProvider.Object));
            
            controller.Trigger(null, "Test1", "Test", new List<string>() { "test.js", "sum.js" });

            mockJobClient.Verify(x => x.Create(
                    It.Is<Job>(i => i.Method.Name == "Execute" && i.Args[1].ToString() == "1"),
                    It.IsAny<EnqueuedState>()));

            mockJobClient.Verify(x => x.Create(
                    It.Is<Job>(i => i.Method.Name == "Execute" && i.Args[1].ToString() == "2"),
                    It.IsAny<EnqueuedState>()));
        }
    }
}