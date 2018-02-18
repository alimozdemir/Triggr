using System;
using System.IO;

namespace Triggr.Infrastructure
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly ScriptStorage _storage;
        private readonly IShellExecutor _shellExecutor;

        public ScriptExecutor(ScriptStorage storage, IShellExecutor shellExecutor)
        {
            _storage = storage;
            _shellExecutor = shellExecutor;
        }

        public string Execute(ProbeType probe, string language, params string[] arg)
        {
            return Execute(probe.ToString(), language, arg);
        }

        public string Execute(string folder, string language, params string[] arg)
        {
            string result = string.Empty;

            var path = _storage.Combine(folder, language);

            var command = $"cd {path} && ./run.sh";

            switch (folder)
            {
                case "CodeChanges":
                    ArgumentCheck(4, arg);
                    break;
                case "AST":
                    ArgumentCheck(3, arg);
                    break;
            }

            command = command + " " + string.Join(" ", arg);
            result = _shellExecutor.Execute(command);

            return result;
        }

        public string ExecuteCommon(string type, params string[] arg)
        {
            string result = string.Empty;
            var path = _storage.Combine("Common");
            
            var command = $"cd {path} && ./{type}.sh";
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