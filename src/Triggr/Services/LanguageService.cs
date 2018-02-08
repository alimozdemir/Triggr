using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Triggr.Services
{
    public class LanguageService : ILanguageService
    {
        public LanguageService(string path)
        {
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                try
                {
                    _languages = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);
                }
                catch (Exception ex)
                {
                    throw new Exception("Unsupported format.", ex);
                }
            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        private static Dictionary<string, string> _languages;

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