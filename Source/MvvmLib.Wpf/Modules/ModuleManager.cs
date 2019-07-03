using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to manage modules.
    /// </summary>
    public class ModuleManager
    {
        private static readonly Dictionary<string, ModuleInfo> modules;
        /// <summary>
        /// The modules discovered.
        /// </summary>
        public static IReadOnlyDictionary<string, ModuleInfo> Modules
        {
            get { return modules; }
        }

        static ModuleManager()
        {
            modules = new Dictionary<string, ModuleInfo>();
            DiscoverModulesInConfiguration();
        }

        private static void DiscoverModulesInConfiguration()
        {
            var config = ConfigurationManager.GetSection("modules") as ModulesConfigurationSection;
            if (config != null)
            {
                foreach (ModuleConfigurationElement module in config.Modules)
                {
                    RegisterModule(module.ModuleName, module.Path, module.ModuleConfigFullName);
                }
            }
        }

        /// <summary>
        /// Registers a module to load on demand.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <param name="path">The path (relative or absolute)</param>
        /// <param name="moduleConfigFullName">The module config full name</param>
        public static void RegisterModule(string moduleName, string path, string moduleConfigFullName)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (moduleConfigFullName == null)
                throw new ArgumentNullException(nameof(moduleConfigFullName));

            var module = new ModuleInfo(moduleName, path, moduleConfigFullName);
            modules[moduleName] = module;
        }

        /// <summary>
        /// Checks if the module is registered.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>True if registered</returns>
        public static bool IsModuleRegistered(string moduleName)
        {
            return modules.ContainsKey(moduleName);
        }

        /// <summary>
        /// Checks if the module is loaded.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>True if loaded</returns>
        public static bool IsModuleLoaded(string moduleName)
        {
            return IsModuleRegistered(moduleName) && modules[moduleName].IsLoaded;
        }

        /// <summary>
        /// Loads the module.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        public static void LoadModule(string moduleName)
        {
            if (modules.TryGetValue(moduleName, out ModuleInfo module))
            {
                if (!module.IsLoaded)
                {
                    var assembly = AssemblyLoader.LoadFile(module.Path);
                    InitializeModule(assembly, module.ModuleConfigFullName);
                    module.isLoaded = true;
                }
            }
        }

        private static void InitializeModule(Assembly assembly, string moduleConfigFullName)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var type = assembly.GetType(moduleConfigFullName);
            if (type == null)
                throw new FileNotFoundException($"Unable to find the module config '{moduleConfigFullName}' for assembly '{assembly.FullName}'");

            var moduleConfig = SourceResolver.CreateInstance(type) as IModuleConfig;
            if (moduleConfig == null)
                throw new ArgumentNullException(nameof(moduleConfig));

            moduleConfig.Initialize();
        }
    }

}
