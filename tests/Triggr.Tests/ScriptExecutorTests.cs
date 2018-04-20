using System;
using System.IO;
using Moq;
using Triggr.Infrastructure;
using Xunit;

namespace Triggr.Tests
{
    public class ScriptExecutorTests : IDisposable
    {
        // Setup
        public ScriptExecutorTests()
        {
            Directory.CreateDirectory("AST/JavaScript");
            Directory.CreateDirectory("StaticAnalysis/JavaScript");
            Directory.CreateDirectory("Common");

            File.Create("AST/JavaScript/run.sh");
            File.Create("StaticAnalysis/JavaScript/run.sh");
            File.Create("Common/run.sh");
        }

        // Teardown
        public void Dispose()
        {
            File.Delete("Common/run.sh");
            File.Delete("AST/JavaScript/run.sh");
            File.Delete("StaticAnalysis/JavaScript/run.sh");


            Directory.Delete("AST/JavaScript");
            Directory.Delete("StaticAnalysis/JavaScript");
            Directory.Delete("Common");
        }

        [Fact]
        public void ExecuteASTWithValidCase()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1"
            };

            string folder = "AST", 
                language = "JavaScript", 
                path = $"{folder}/{language}", 
                command = $"cd {path} && ./run.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder, language)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            var result = executor.Execute(folder, language, args);

            Assert.Equal("1", result);
        }

        [Fact]
        public void ExecuteASTWithInalidArgumentSize()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1", "1"
            };

            string folder = "AST",
                language = "JavaScript",
                path = $"{folder}/{language}",
                command = $"cd {path} && ./run.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder, language)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            Action act = () => executor.Execute(folder, language, args);

            Assert.ThrowsAny<Exception>(act);
        }

        [Fact]
        public void ExecuteASTWithFileNotFound()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1"
            };

            string folder = "AST",
                language = "CSharp",
                path = $"{folder}/{language}",
                command = $"cd {path} && ./run.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder, language)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            var result = executor.Execute(folder, language, args);

            Assert.Equal("-1", result);
        }

        [Fact]
        public void ExecuteCommonWithValid()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1"
            };

            string folder = "Common",
                path = $"{folder}/",
                type = "run",
                command = $"cd {path} && ./{type}.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            var result = executor.ExecuteCommon(type, args);

            Assert.Equal("1", result);
        }

        [Fact]
        public void ExecuteProbeWithValidCase()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1"
            };

            string folder = ProbeType.StaticAnalysis.ToString(),
                language = "JavaScript",
                path = $"{folder}/{language}",
                command = $"cd {path} && ./run.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder, language)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            var result = executor.Execute(ProbeType.StaticAnalysis, language, args);

            Assert.Equal("1", result);
        }

        [Fact]
        public void ExecuteProbeWithFileNotFound()
        {
            var mockShellExecutor = new Mock<IShellExecutor>();
            var mockScriptStorage = new Mock<ScriptStorage>();

            var args = new string[] {
                "1", "1", "1"
            };

            string folder = ProbeType.StaticAnalysis.ToString(),
                language = "CSharp",
                path = $"{folder}/{language}",
                command = $"cd {path} && ./run.sh " + string.Join(" ", args);

            mockScriptStorage.Setup(i => i.Combine(folder, language)).Returns(path);
            mockShellExecutor.Setup(i => i.Execute(command)).Returns("1");

            ScriptExecutor executor = new ScriptExecutor(mockScriptStorage.Object, mockShellExecutor.Object);

            var result = executor.Execute(ProbeType.StaticAnalysis, language, args);

            Assert.Equal("-1", result);
        }
    }
}