using System.IO;
using System.Reflection;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to load assemblies.
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// Loads the assembly.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The assembly</returns>
        public static Assembly LoadFile(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
                throw new FileNotFoundException($"No file found for the path '{path}'");

            var assembly = Assembly.LoadFile(file.FullName); // can fail, unexpected file for example
            return assembly;
        }
    }
}
