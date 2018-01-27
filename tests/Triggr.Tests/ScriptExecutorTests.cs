using System;
using System.IO;
using Triggr.Services;
using Xunit;

namespace Triggr.Tests
{
    public class ScriptExecutorTests
    {
        [Fact]
        public void Basic()
        {
            ScriptStorage storage = new ScriptStorage("../../../../../src/Scripts/", false);
            Assert.Equal(Directory.Exists(storage.Path), true);

            ScriptExecutor scriptExecutor = new ScriptExecutor(storage);

            var result = scriptExecutor.Execute(ProbeType.CodeChanges, "JavaScript", "1", "2", "3", "4");

            Assert.NotEmpty(result);
        }
    }
}