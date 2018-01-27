using System.IO;

namespace Triggr.Services
{
    public class ScriptExecutor : IScriptExecutor
    {
        private readonly ScriptStorage _storage;
        public ScriptExecutor(ScriptStorage storage)
        {
            _storage = storage;
        }

        public string Execute(ProbeType probe, string language, params string[] arg)
        {
            string result = string.Empty;
            string type = probe.ToString();
            var path = _storage.Combine(type, language);

            var command = $"cd {path} && ./run.sh";

            switch (probe)
            {
                case ProbeType.CodeChanges:

                    if (arg.Length == 4)
                    {
                        command = command + " " + string.Join(" ", arg);
                    }
                    else
                    {
                        //throw exception
                    }
                    break;
            }

            result = command.Bash();

            return result;
        }

    }
}