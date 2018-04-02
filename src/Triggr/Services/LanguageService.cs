using System;
using System.Collections.Generic;
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
                try
                {
                    _languages = JsonConvert.DeserializeObject<Dictionary<string, LanguageProperties>>(text);
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