using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Triggr.Providers;

namespace Triggr
{
    public sealed class Container
    {
        public Container(string name, string folder, Data.Repository repository)
        {
            Name = name;
            Folder = folder;
            Repository = repository;
        }
        public Data.Repository Repository { get; }
        public string Name { get; }
        public string Folder { get; }
        private string TriggrFolder => Path.Combine(Folder, ConstTriggrFolder);
        public string Languages { get; set; }
        public DateTimeOffset UpdatedTime { get; set; }


        private const string ConstTriggrFolder = ".triggr";

        public bool IsTriggrProject()
        {
            return Directory.Exists(TriggrFolder);
        }

        public List<Probe> CheckForProbes()
        {
            List<Probe> result = new List<Probe>();
            var probeFiles = Directory.GetFiles(TriggrFolder, "*.json");

            foreach (var path in probeFiles)
            {
                var content = File.ReadAllText(path);
                var probe = JsonConvert.DeserializeObject<Probe>(content);

                result.Add(probe);
            }

            return result;
        }

        public bool Update(IProvider provider)
        {
            bool result = false;

            var path = provider.Update(this.Repository);

            if (string.IsNullOrEmpty(path)) // no update
            {

            }
            else
            {
                
            }

            return result;
        }
    }
}