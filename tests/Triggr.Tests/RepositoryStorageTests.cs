using System;
using System.IO;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{
    public class RepositoryStorageTests
    {
        [Fact]
        public void Ctor_RelativePath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            Assert.Equal(storage.Path, "fakepath");
        }

        [Fact]
        public void Ctor_AbsolutePath()
        {
            var storage = new RepositoryStorage("fakepath");
            Assert.Equal(storage.Path, Path.Combine(Environment.CurrentDirectory, "fakepath"));
        }

        [Fact]
        public void Ctor_EmptyString()
        {
            Action cons = () => new RepositoryStorage(string.Empty);
            Assert.ThrowsAny<ArgumentException>(cons);
        }

        [Fact]
        public void SetPath_RelativePath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            storage.Set("fakepath2", false);
            Assert.Equal(storage.Path, "fakepath2");
        }

        [Fact]
        public void SetPath_AbsolutePath()
        {
            var storage = new RepositoryStorage("fakepath", true);
            storage.Set("fakepath2", true);
            Assert.Equal(storage.Path, Path.Combine(Environment.CurrentDirectory, "fakepath2"));
        }

        [Fact]
        public void SetPath_EmptyString()
        {
            var storage = new RepositoryStorage("fakepath", true);

            Action act = () => storage.Set("", true);
            Assert.ThrowsAny<ArgumentException>(act);
        }

        [Fact]
        public void Combine_OnePath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            
            var path = storage.Combine("s1");

            Assert.Equal(path, Path.Combine("fakepath", "s1"));
        }
        [Fact]
        public void Combine_TwoPath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            
            var path = storage.Combine("s1", "s2");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2"));
        }

        [Fact]
        public void Combine_ThreePath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            
            var path = storage.Combine("s1", "s2", "s3");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2", "s3"));
        }

        [Fact]
        public void Combine_FourPath()
        {
            var storage = new RepositoryStorage("fakepath", false);
            
            var path = storage.Combine("s1", "s2", "s3", "s4");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2", "s3", "s4"));
        }
    }
}