namespace Triggr.Infrastructure
{
    public class WindowsExecutor : IShellExecutor
    {
        public string Extension => ".bat";

        public string Execute(string command)
        {
            throw new System.NotImplementedException();
        }
    }
}