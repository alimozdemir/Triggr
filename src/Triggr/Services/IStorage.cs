namespace Triggr.Services
{
    public interface IStorage
    {
        void Set(string path);
        void Set(string path, bool environmentPath);
        string Combine(string path);
        string Path { get; }
    }
}