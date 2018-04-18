using System.Collections.Generic;
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
        public void TestName()
        {
            var repo = new Repository() { Name = "Test1", Id = "1", OwnerName = "Test", Provider = "Git" };
            var data = new List<Repository>()
            {
                repo
            }.AsQueryable();

            var mockDb = GetDatabaseMocks(data);
            mockDb.mockContext.Setup(i => i.Repositories).Returns(mockDb.mockSet.Object);

            var probes = new List<Probe>(){
                new Probe() { Id = "1", Object = new ObjectInformation() { Path = "test.js" } }
            };

            var mockProviderFactory = new Mock<IProviderFactory>();
            var mockLanguageService = new Mock<ILanguageService>();
            var mockScriptExecutor = new Mock<IScriptExecutor>();
            var mockContainerService = new Mock<IContainerService>();
            var mockContainer = new Mock<Container>();
            var mockProvider = new Mock<IProvider>();

            var language = new LanguageProperties()
            {
                FolderName = "JavaScript",
                FullProject = false
            };

            mockContainer.Setup(i => i.CheckForProbes()).Returns(probes);

            mockContainerService.Setup(i => i.GetContainer("1"))
                                .Returns(mockContainer.Object);

            mockLanguageService.Setup(i => i.Define("test.js"))
                               .Returns(language);

            mockProviderFactory.Setup(i => i.GetProvider(repo.Provider))
                                .Returns(mockProvider.Object);

            var control = new ProbeControl(mockProviderFactory.Object,
                                            mockLanguageService.Object,
                                            mockScriptExecutor.Object,
                                            mockDb.mockContext.Object,
                                            mockContainerService.Object);



            control.Execute(null, "1", "1");

        }
    }
}