using System;
using Xunit;
using Triggr.Services;

namespace Triggr.Tests
{
    public class LanguageServiceTests
    {
        [Fact]
        public void DefineWithFileNameJs()
        {
            LanguageService service = new LanguageService();
            var result = service.Define("test.js");
            Assert.Equal(result, "JavaScript");
        }

        [Fact]
        public void DefineWithFilePathJs()
        {
            LanguageService service = new LanguageService();
            var result = service.Define("/path/test.js");
            Assert.Equal(result, "JavaScript");
        }

        [Fact]
        public void DefineWithFilePathCs()
        {
            LanguageService service = new LanguageService();
            var result = service.Define("/path/test.cs");
            Assert.Equal(result, "CSharp");
        }

        [Fact]
        public void DefineWithFileNameCs()
        {
            LanguageService service = new LanguageService();
            var result = service.Define("test.cs");
            Assert.Equal(result, "CSharp");
        }

    }
}
