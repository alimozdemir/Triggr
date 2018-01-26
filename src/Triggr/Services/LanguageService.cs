using System;
using System.Collections.Generic;
using System.IO;

namespace Triggr.Services
{
    public class LanguageService : ILanguageService
    {
        private static Dictionary<string, string> _languages = new Dictionary<string, string>()
        {
            {".js", "JavaScript"},
            {".cs", "CSharp"}
        };
        public string Define(string path)
        {
            var result = string.Empty;
            FileInfo info = new FileInfo(path);

            if (!_languages.TryGetValue(info.Extension, out result))
                throw new Exception("Unsupported programming language.");

            return result;
        }
    }
}