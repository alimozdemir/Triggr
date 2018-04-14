using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Triggr.Providers;

namespace Triggr
{
    public class Container
    {
        public Container()
        {
            
        }
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
        public DateTimeOffset UpdatedTime { get; set; }

        private const string ConstTriggrFolder = ".triggr";

        public bool IsTriggrProject()
        {
            return Directory.Exists(TriggrFolder);
        }

        public virtual List<Probe> CheckForProbes()
        {
            List<Probe> result = new List<Probe>();
            var probeFiles = Directory.GetFiles(TriggrFolder, "*.json");

            foreach (var path in probeFiles)
            {
                var content = File.ReadAllText(path);

                var probe = JsonConvert.DeserializeObject<Probe>(content);
                if (!string.IsNullOrEmpty(probe.Id) && probe.Object != null)
                {
                    result.Add(probe);
                }

            }

            return result;
        }

        public virtual IEnumerable<string> Update(IProvider provider)
        {
            var path = provider.Update(this.Repository);

            if (string.IsNullOrEmpty(path)) // no update
            {
                return Enumerable.Empty<string>();
            }
            else
            {
                List<string> result = new List<string>();

                var files = provider.LastUpdatedFiles(this.Repository);

                var entities = files.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var file in entities)
                {
                    result.Add(file);
                }

                return result;
            }
        }
    }
}