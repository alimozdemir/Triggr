using System.Threading.Tasks;
using Triggr.Data;

namespace Triggr.Providers
{
    /// <summary>
    /// SCM Provider interface
    /// </summary>
    public interface IProvider
    {
        bool Restore(Data.Repository data, string fileName, bool previousFile);
        /// <summary>
        /// Return last update's file lists
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>List of files as one string</returns>
        string LastUpdatedFiles(Data.Repository data);
        /// <summary>
        /// Updates the repository on local storage
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>The path of repository</returns>
        string Update(Repository repository);
        /// <summary>
        /// Controls the repository is exist in local storage
        /// </summary>
        /// <param name="repository"></param>
        /// <returns></returns>
        bool Exist(Repository repository);
        /// <summary>
        /// Clones the repository from source control to local storage (e.g. git clone url)
        /// </summary>
        /// <param name="repository"></param>
        /// <returns>The path of repository</returns>
        string Clone(Repository repository);
        /// <summary>
        /// Checks if given url is valid with that provider
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        bool IsValid(string url);

        bool WebhookRegister();
        
        string GetProviderType { get; }
    }
}