using Xunit;
using System.IO;
using System;
using System.Linq;
using Moq;
using Triggr.Providers;

namespace Triggr.Tests
{
    public class ContainerTests
    {
        private string GetPath(string sub) => Path.Combine("../../../Fixtures/repositories", sub);
        // Setup TODO
        [Fact]
        public void IsTriggrProjectValidPath()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo1"), data);
            Assert.Equal("Title", container.Name);
            Assert.True(container.IsTriggrProject());
        }

        [Fact]
        public void IsTriggrProjectInvalidPath()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo2"), data);

            Assert.False(container.IsTriggrProject());
        }

        [Fact]
        public void CheckForProbesWithInvalidPath()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo2"), data);
            Action action = () => container.CheckForProbes();
            Assert.ThrowsAny<DirectoryNotFoundException>(action);
        }

        [Fact]
        public void CheckForProbesWithValidPathStaticProbe()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo1"), data);
            var probes = container.CheckForProbes();
            var first = probes.FirstOrDefault();

            Assert.NotNull(first);
            Assert.NotNull(first.Id);
            Assert.NotNull(first.Object);
            Assert.Equal(ProbeType.StaticAnalysis, first.ProbeType);
            Assert.NotNull(first.Metrics);
        }

        [Fact]
        public void CheckForProbesWithValidPathCodeChangesProbe()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo3"), data);
            var probes = container.CheckForProbes();
            var first = probes.FirstOrDefault();

            Assert.NotNull(first);
            Assert.NotNull(first.Id);
            Assert.NotNull(first.Object);
            Assert.Equal(ProbeType.CodeChanges, first.ProbeType);
        }

        [Fact]
        public void CheckForProbesWithValidPathNoProbe()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo4"), data);
            var probes = container.CheckForProbes();
            var first = probes.FirstOrDefault();

            Assert.Null(first);
        }

        [Fact]
        public void CheckForProbesWithValidPathProblematicProbe()
        {
            var data = new Data.Repository();

            Container container = new Container("Title", GetPath("repo5"), data);
            var probes = container.CheckForProbes();
            var first = probes.FirstOrDefault();

            Assert.Null(first);
        }

        [Fact]
        public void UpdateWithNoUpdate()
        {
            var data = new Data.Repository();
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(i => i.Update(data)).Returns(string.Empty);
            Container container = new Container("Title", GetPath("repo1"), data);

            var result = container.Update(mockProvider.Object);
            
            Assert.Equal(0, result.Count());
        }


        [Fact]
        public void UpdateWithFileList()
        {
            var data = new Data.Repository();
            var mockProvider = new Mock<IProvider>();
            mockProvider.Setup(i => i.Update(data)).Returns("/");
            mockProvider.Setup(i => i.LastUpdatedFiles(data)).Returns($"test.js{Environment.NewLine}hello.js2{Environment.NewLine}{Environment.NewLine}");
            Container container = new Container("Title", GetPath("repo1"), data);

            var result = container.Update(mockProvider.Object);

            Assert.Equal(2, result.Count());            
            Assert.Contains("test.js", result);
            Assert.Contains("hello.js2", result);
        }
    }
}