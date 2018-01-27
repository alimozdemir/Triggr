namespace Triggr.Services
{
    public interface IStorage
    {
        void Set(string path);
        void Set(string path, bool environmentPath);
        string Combine(string path);
        string Combine(string path1, string path2);
        string Combine(string path1, string path2, string path3);
        string Combine(string path1, string path2, string path3, string path4);
        string Path { get; }
    }
}