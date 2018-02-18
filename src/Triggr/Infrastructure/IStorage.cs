namespace Triggr.Infrastructure
{
    public interface IStorage
    {
        /// <summary>
        /// Sets the path of storage
        /// </summary>
        /// <param name="path">Starting path</param>
        void Set(string path);
        /// <summary>
        /// Sets the path of storage
        /// </summary>
        /// <param name="path">Starting path</param>
        /// <param name="environmentPath">Absolute path or not</param>
        void Set(string path, bool environmentPath);
        /// <summary>
        /// Combine Path property with given parameters.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string Combine(string path);
        string Combine(string path1, string path2);
        string Combine(string path1, string path2, string path3);
        string Combine(string path1, string path2, string path3, string path4);
        /// <summary>
        /// Current path
        /// </summary>
        /// <returns></returns>
        string Path { get; }
    }
}