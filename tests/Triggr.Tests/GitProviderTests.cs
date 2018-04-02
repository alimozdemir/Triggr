using Triggr.Providers;
using Xunit;
using Moq;
using Triggr.Infrastructure;
using Triggr.Data;
using System.IO;

namespace Triggr.Tests
{
    public class GitProviderTests
    {
        [Fact]
        public void CloneRepositoryTest()
        {
            var storageMock = new Mock<RepositoryStorage>();
            var executorMock = new Mock<ScriptExecutor>();
            var gitMock = new Mock<LibGit2SharpWrapper>();

            var provider = new GitProvider(storageMock.Object, executorMock.Object, gitMock.Object);

            Repository repository = new Repository();
            repository.Id = "1";
            repository.Url = "fakeurl";
            var path = Path.Combine("/", repository.Id);
            storageMock.Setup(i => i.Combine(repository.Id)).Returns(path);
            gitMock.Setup(i => i.Clone(repository.Url, path)).Returns("");

            var result = provider.Clone(repository);

            Assert.Empty(result);
        }

        [Fact]
        public void LastUpdatedFilesTest()
        {
            var storageMock = new Mock<RepositoryStorage>();
            var executorMock = new Mock<ScriptExecutor>();
            var gitMock = new Mock<LibGit2SharpWrapper>();

            var provider = new GitProvider(storageMock.Object, executorMock.Object, gitMock.Object);
            
            Repository repository = new Repository();
            repository.Id = "1";
            repository.Url = "fakeurl";
            var path = Path.Combine("/", repository.Id);
            storageMock.Setup(i => i.Combine(repository.Id)).Returns(path);
            executorMock.Setup(i => i.ExecuteCommon("LastUpdatedFiles", path)).Returns("");

            var result = provider.LastUpdatedFiles(repository);

            Assert.Empty(result);
        }
    }
}