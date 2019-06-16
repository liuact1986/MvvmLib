using System;
using System.Reflection;

namespace MvvmLib.Modules
{

    /// <summary>
    /// Allows to load and initialize modules.
    /// </summary>
    public class AssemblyLoader
    {
        /// <summary>
        /// Load the module for the file.
        /// </summary>
        /// <param name="file">The file</param>
        /// <returns>The Assembly</returns>
        public static Assembly Load(string file)
        {
            var assembly = Assembly.LoadFile(file);
            return assembly;
        }

        /// <summary>
        /// Creates the <see cref="IModuleConfig"/> and invokes <see cref="IModuleConfig.Initialize"/>.
        /// </summary>
        /// <param name="assembly">The module</param>
        /// <param name="moduleConfigFullName">The module config full name</param>
        public static void InitializeModule(Assembly assembly, string moduleConfigFullName)
        {
            var type = assembly.GetType(moduleConfigFullName);
            if (type == null)
                throw new ModuleLoadingFailException($"Unable to find the module config '{moduleConfigFullName}' for assembly '{assembly.FullName}'");

            var moduleConfig = Activator.CreateInstance(type) as IModuleConfig;
            moduleConfig.Initialize();
        }
    }

}
