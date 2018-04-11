using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ContainerServiceTests
    {
        [Fact]
        public void TestName()
        {
            var context = new Mock<TriggrContext>();
            var set = new Mock<DbSet<Repository>>();
            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();
            var repo = new Repository { Id = "1", Name = "Test", Provider = "Git" };

            var data = new List<Repository>
            {
                repo
            }.AsQueryable();

            set.As<IQueryable<Repository>>().Setup(m => m.Provider).Returns(data.Provider);
            set.As<IQueryable<Repository>>().Setup(m => m.Expression).Returns(data.Expression);
            set.As<IQueryable<Repository>>().Setup(m => m.ElementType).Returns(data.ElementType);
            set.As<IQueryable<Repository>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            set.Setup(i => i.Find("1")).Returns(repo);

            context.Setup(i => i.Repositories).Returns(set.Object);
            provider.Setup(i => i.Exist(repo)).Returns(false);
            provider.Setup(i => i.Clone(repo)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(provider.Object);
            var service = new ContainerService(context.Object, providerFactory.Object, storage.Object);

            var result = service.GetContainer("1");

            provider.Verify(i => i.Exist(repo));
            provider.Verify(i => i.Clone(repo));

        }
        [Fact]
        public void TestName2()
        {
            var set = new Mock<DbSet<Repository>>();
            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();
            var repo = new Repository { Id = "1", Name = "Test", Provider = "Git" };

            var data = new List<Repository>
            {
                repo
            }.AsQueryable();
            
            set.As<IAsyncEnumerable<Repository>>()
               .Setup(d => d.GetEnumerator())
               .Returns(new AsyncEnumerator<Repository>(data.GetEnumerator()));

            set.As<IQueryable<Repository>>().Setup(m => m.Provider).Returns(data.Provider);
            set.As<IQueryable<Repository>>().Setup(m => m.Expression).Returns(data.Expression);
            set.As<IQueryable<Repository>>().Setup(m => m.ElementType).Returns(data.ElementType);
            set.As<IQueryable<Repository>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            //set.As<IQueryable<Repository>>().Setup(m => m.ToListAsync()).ReturnsAsync(data.ToList());

            var context = new Mock<TriggrContext>();
            context.Setup(i => i.Repositories).Returns(set.Object);

            provider.Setup(i => i.Exist(repo)).Returns(false);
            provider.Setup(i => i.Clone(repo)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(provider.Object);
            var service = new ContainerService(context.Object, providerFactory.Object, storage.Object);

            var result = service.CheckAsync();

            provider.Verify(i => i.Exist(repo));
            provider.Verify(i => i.Clone(repo));

        }

    }
}