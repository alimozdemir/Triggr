using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using Triggr.Infrastructure;

namespace Triggr.Services
{
    public class LanguageService : ILanguageService
    {
        private static Dictionary<string, LanguageProperties> _languages;
        public LanguageService(string path)
        {
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);

                _languages = JsonConvert.DeserializeObject<Dictionary<string, LanguageProperties>>(text);
                
                if (_languages == null 
                    || _languages.Count == 0 
                    || _languages.All(i => i.Value == null || (i.Value != null && string.IsNullOrEmpty(i.Value.FolderName))))
                {
                    throw new Exception("No language found.");
                }

            }
            else
            {
                throw new Exception("File not found.");
            }
        }

        public Dictionary<string, LanguageProperties> Languages => _languages;

        public LanguageProperties Define(string path)
        {
            var result = new LanguageProperties();
            FileInfo info = new FileInfo(path);

            if (!_languages.TryGetValue(info.Extension, out result))
                throw new Exception("Unsupported programming language.");

            return result;
        }
    }
}