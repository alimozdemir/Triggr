using System.Collections.Generic;

namespace Triggr.UI.Models
{
    public class GithubPushModel
    {
        public string Ref { get; set; }
        public string Before { get; set; }

        public List<Commit> Commits { get; set; }
        public Repository Repository { get; set; }
    }

    public class Commit
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public List<string> Modified { get; set; }
    }

    public class Repository
    {
        public string Name { get; set; }
        public Owner Owner { get; set; }

    }

    public class Owner
    {
        public string Name { get; set; }
    }
}