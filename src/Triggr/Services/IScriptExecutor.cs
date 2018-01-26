namespace Triggr.Services
{
    public interface IScriptExecutor
    {
        string Execute(Probe probe, string command, params string[] arg);
    }
}