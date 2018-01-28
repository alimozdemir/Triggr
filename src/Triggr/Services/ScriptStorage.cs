using System;

namespace Triggr.Services
{
    // it should be an abstract class
    public class ScriptStorage : IStorage
    {
        private string _path;
        public string Path => _path;
        public ScriptStorage(string path)
        {
            Set(path);
        }

        public ScriptStorage(string path, bool environmentPath)
        {
            Set(path, environmentPath);
        }
        public void Set(string path)
        {
            Set(path, true);
        }

        public void Set(string path, bool environmentPath)
        {
            _path = environmentPath ? System.IO.Path.Combine(Environment.CurrentDirectory, path) : path;
        }
        public string Combine(string path)
        {
            return System.IO.Path.Combine(_path, path);
        }

        public string Combine(string path1, string path2)
        {
            return System.IO.Path.Combine(_path, path1, path2);
        }
        public string Combine(string path1, string path2, string path3)
        {
            return System.IO.Path.Combine(_path, path1, path2, path3);
        }
        public string Combine(string path1, string path2, string path3, string path4)
        {
            return System.IO.Path.Combine(_path, path1, path2, path3, path4);
        }
    }
}