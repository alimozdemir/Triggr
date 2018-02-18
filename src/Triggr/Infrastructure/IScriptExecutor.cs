namespace Triggr.Infrastructure
{
    public interface IScriptExecutor
    {
        string Execute(ProbeType probe, string language, params string[] arg);
        string Execute(string folder, string language, params string[] arg);
        string ExecuteCommon(string type, params string[] arg);
    }
}