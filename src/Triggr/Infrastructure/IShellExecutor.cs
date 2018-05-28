namespace Triggr.Infrastructure
{
    public interface IShellExecutor
    {
        string Execute(string command);
        string RunFile { get; }
    }
}