namespace Triggr.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// Defines a programming language with given a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string Define(string path);
    }
}