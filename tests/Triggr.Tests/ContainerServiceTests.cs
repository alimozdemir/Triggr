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
        public void GetContainerWithClone()
        {
            var repo = new Repository { Id = "1", Name = "Test", Provider = "Git" };

            var data = new List<Repository>
            {
                repo
            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();
            var db = GetDatabaseMocks(data);

            db.mockSet.Setup(i => i.Find("1")).Returns(repo);
            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            provider.Setup(i => i.Exist(repo)).Returns(false);
            provider.Setup(i => i.Clone(repo)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(provider.Object);
            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = service.GetContainer("1");

            provider.Verify(i => i.Exist(repo));
            provider.Verify(i => i.Clone(repo));
            Assert.NotNull(result);
            Assert.NotNull(result.UpdatedTime);
            Assert.Equal(result.Folder, "/1");
            Assert.Equal(repo, result.Repository);
            
        }


        [Fact]
        public void GetContainerNotFound()
        {
            var data = new List<Repository>
            {
                
            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();
            var db = GetDatabaseMocks(data);

            db.mockSet.Setup(i => i.Find("1"));
            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = service.GetContainer("1");

            Assert.Null(result);
        }

        [Fact]
        public void GetContainerWithCombine()
        {
            var repo = new Repository { Id = "1", Name = "Test", Provider = "Git" };

            var data = new List<Repository>
            {
                repo
            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();
            var db = GetDatabaseMocks(data);

            db.mockSet.Setup(i => i.Find("1")).Returns(repo);
            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            provider.Setup(i => i.Exist(repo)).Returns(true);
            storage.Setup(i => i.Combine(repo.Id)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider(repo.Provider)).Returns(provider.Object);
            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = service.GetContainer("1");

            provider.Verify(i => i.Exist(repo));
            storage.Verify(i => i.Combine(repo.Id));

            Assert.NotNull(result);
            Assert.NotNull(result.UpdatedTime);
            Assert.Equal(result.Folder, "/1");
            Assert.Equal(repo, result.Repository);
        }


        [Fact]
        public async Task CheckAsyncWithCloneMultipleRepositories()
        {
            var repo1 = new Repository { Id = "1", Name = "Test1", Provider = "Git" };
            var repo2 = new Repository { Id = "2", Name = "Test2", Provider = "Git" };

            var data = new List<Repository>
            {
                repo1, repo2
            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();

            var db = GetDatabaseMocks(data);

            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            provider.Setup(i => i.Exist(repo1)).Returns(false);
            provider.Setup(i => i.Exist(repo2)).Returns(false);
            provider.Setup(i => i.Clone(repo1)).Returns("/1");
            provider.Setup(i => i.Clone(repo2)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider("Git")).Returns(provider.Object);

            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = await service.CheckAsync();

            provider.Verify(i => i.Exist(repo1));
            provider.Verify(i => i.Clone(repo1));
            provider.Verify(i => i.Exist(repo2));
            provider.Verify(i => i.Clone(repo2));

            var resultAsList = result.ToList();
            Assert.Equal(2, resultAsList.Count);
            Assert.Equal("Container #1", resultAsList[0].Name);
            Assert.Equal("Container #2", resultAsList[1].Name);
            Assert.Equal(repo1, resultAsList[0].Repository);
            Assert.Equal(repo2, resultAsList[1].Repository);
        }

        [Fact]
        public async Task CheckAsyncWithCombineMultipleRepositories()
        {
            var repo1 = new Repository { Id = "1", Name = "Test1", Provider = "Git" };
            var repo2 = new Repository { Id = "2", Name = "Test2", Provider = "Git" };

            var data = new List<Repository>
            {
                repo1, repo2
            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();

            var db = GetDatabaseMocks(data);

            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            provider.Setup(i => i.Exist(repo1)).Returns(true);
            provider.Setup(i => i.Exist(repo2)).Returns(true);
            storage.Setup(i => i.Combine(repo1.Id)).Returns("/1");
            storage.Setup(i => i.Combine(repo1.Id)).Returns("/1");

            providerFactory.Setup(i => i.GetProvider("Git")).Returns(provider.Object);

            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = await service.CheckAsync();

            provider.Verify(i => i.Exist(repo1));
            provider.Verify(i => i.Exist(repo2));
            storage.Verify(i => i.Combine(repo1.Id));
            storage.Verify(i => i.Combine(repo2.Id));

            var resultAsList = result.ToList();
            Assert.Equal(2, resultAsList.Count);
            Assert.Equal("Container #1", resultAsList[0].Name);
            Assert.Equal("Container #2", resultAsList[1].Name);
            Assert.Equal(repo1, resultAsList[0].Repository);
            Assert.Equal(repo2, resultAsList[1].Repository);
        }

        [Fact]
        public async Task CheckAsyncWithNoRepositories()
        {
            var data = new List<Repository>
            {

            }.AsQueryable();

            var providerFactory = new Mock<IProviderFactory>();
            var provider = new Mock<IProvider>();
            var storage = new Mock<RepositoryStorage>();

            var db = GetDatabaseMocks(data);

            db.mockContext.Setup(i => i.Repositories).Returns(db.mockSet.Object);

            var service = new ContainerService(db.mockContext.Object, providerFactory.Object, storage.Object);

            var result = await service.CheckAsync();

            var resultAsList = result.ToList();
            Assert.Equal(0, resultAsList.Count);
            
        }
    }
}