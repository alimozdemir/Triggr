using System.Diagnostics;

namespace Triggr.Infrastructure
{
    public class UnixExecutor : IShellExecutor
    {
        public string Execute(string cmd)
        {
            // reference : https://loune.net/2017/06/running-shell-bash-commands-in-net-core/
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            
            process.Start();
            string result = process.StandardOutput.ReadToEnd();

            if (string.IsNullOrEmpty(result))
                result = process.StandardError.ReadToEnd();
                
            process.WaitForExit();

            return result;
        }
    }
}