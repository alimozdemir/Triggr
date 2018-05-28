using System;
using System.IO;

namespace Triggr.Infrastructure
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly ScriptStorage _storage;
        private readonly IShellExecutor _shellExecutor;
        public ScriptExecutor()
        {
            
        }
        public ScriptExecutor(ScriptStorage storage, IShellExecutor shellExecutor)
        {
            _storage = storage;
            _shellExecutor = shellExecutor;
        }

        public virtual string Execute(ProbeType probe, string language, params string[] arg)
        {
            return Execute(probe.ToString(), language, arg);
        }

        public virtual string Execute(string folder, string language, params string[] arg)
        {
            string result = "-1";

            var path = _storage.Combine(folder, language);
            if (File.Exists(Path.Combine(path, $"run{_shellExecutor.Extension}")))
            {
                var command = $"cd {path} && ./run{_shellExecutor.Extension}";

                if(folder.Equals("AST"))
                    ArgumentCheck(3, arg);
               
                command = command + " " + string.Join(" ", arg);
                result = _shellExecutor.Execute(command);
            }
            
            return result;
        }

        public virtual string ExecuteCommon(string type, params string[] arg)
        {
            string result = "-1";
            var path = _storage.Combine("Common");

            var command = $"cd {path} && ./{type}{_shellExecutor.Extension}";
            command = command + " " + string.Join(" ", arg);
            //todo: more consistent way
            result = _shellExecutor.Execute(command);

            return result;
        }

        private void ArgumentCheck(int value, string[] args)
        {
            if (args.Length != value)
                throw new Exception("Not enough arguments.");
        }
    }
}