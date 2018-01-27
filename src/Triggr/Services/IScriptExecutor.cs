namespace Triggr.Services
{
    public interface IScriptExecutor
    {
        string Execute(ProbeType probe, string language, params string[] arg);
    }
}