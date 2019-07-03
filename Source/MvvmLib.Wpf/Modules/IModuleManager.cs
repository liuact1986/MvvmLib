using System;
using System.Collections.Generic;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to manage modules loaded on demand.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// The modules.
        /// </summary>
        IReadOnlyDictionary<string, ModuleInfo> Modules { get; }

        /// <summary>
        /// Invoked on module loaded.
        /// </summary>
        event EventHandler<ModuleLoadedEventArgs> ModuleLoaded;

        /// <summary>
        /// Checks if the module is loaded.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>True if loaded</returns>
        bool IsModuleLoaded(string moduleName);

        /// <summary>
        /// Loads the module.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        void LoadModule(string moduleName);

        /// <summary>
        /// Registers a module to load on demand.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <param name="path">The path (relative or absolute)</param>
        /// <param name="moduleConfigurationFullName">The module configuration full name</param>
        void RegisterModule(string moduleName, string path, string moduleConfigurationFullName);
    }
}
