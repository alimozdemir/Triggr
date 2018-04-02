using System;
using System.Collections.Generic;
using System.IO;
using Triggr.Infrastructure;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{

    public class StorageConfig
    {
        public static IEnumerable<object[]> TestCases
        {
            get
            {
                var items = new List<object[]>();

                items.Add(new object[] { new RepositoryStorage("fakepath", false) });
                items.Add(new object[] { new ScriptStorage("fakepath", false) });

                return items;
            }
        }
    }
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

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void SetPath_RelativePath(IStorage storage)
        {
            storage.Set("fakepath2", false);
            Assert.Equal(storage.Path, "fakepath2");
        }

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void SetPath_AbsolutePath(IStorage storage)
        {
            storage.Set("fakepath2", true);
            Assert.Equal(storage.Path, Path.Combine(Environment.CurrentDirectory, "fakepath2"));
        }

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void SetPath_EmptyString(IStorage storage)
        {

            Action act = () => storage.Set("", true);
            Assert.ThrowsAny<ArgumentException>(act);
        }

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void Combine_OnePath(IStorage storage)
        {
            var path = storage.Combine("s1");

            Assert.Equal(path, Path.Combine("fakepath", "s1"));
        }
        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void Combine_TwoPath(IStorage storage)
        {
            var path = storage.Combine("s1", "s2");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2"));
        }

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void Combine_ThreePath(IStorage storage)
        {
            var path = storage.Combine("s1", "s2", "s3");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2", "s3"));
        }

        [Theory]
        [MemberData(nameof(StorageConfig.TestCases), MemberType = typeof(StorageConfig))]
        public void Combine_FourPath(IStorage storage)
        {   
            var path = storage.Combine("s1", "s2", "s3", "s4");

            Assert.Equal(path, Path.Combine("fakepath", "s1", "s2", "s3", "s4"));
        }
    }
}