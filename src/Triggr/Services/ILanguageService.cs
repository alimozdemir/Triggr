using System.Collections.Generic;
using Triggr.Infrastructure;

namespace Triggr.Services
{
    public interface ILanguageService
    {
        /// <summary>
        /// Defines a programming language with given a path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        LanguageProperties Define(string path);

        Dictionary<string, LanguageProperties> Languages { get; }
    }
}