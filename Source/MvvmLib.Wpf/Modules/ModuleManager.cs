using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to manage modules loaded on demand.
    /// </summary>
    public class ModuleManager : IModuleManager
    {
        private readonly Dictionary<string, ModuleInfo> modules;
        /// <summary>
        /// The modules.
        /// </summary>
        public IReadOnlyDictionary<string, ModuleInfo> Modules
        {
            get { return modules; }
        }

        /// <summary>
        /// Invoked on module loaded.
        /// </summary>
        public event EventHandler<ModuleLoadedEventArgs> ModuleLoaded;

        private static IModuleManager _default = new ModuleManager();
        /// <summary>
        /// The default instance of <see cref="ModuleManager"/>.
        /// </summary>
        public static IModuleManager Default
        {
            get { return _default; }
        }

        /// <summary>
        /// Creates the <see cref="ModuleManager"/>.
        /// </summary>
        public ModuleManager()
        {
            modules = new Dictionary<string, ModuleInfo>();
            DiscoverModulesInConfigurationFile();
        }

        private void DiscoverModulesInConfigurationFile()
        {
            var config = ConfigurationManager.GetSection("modules") as ModulesConfigurationSection;
            if (config != null)
            {
                foreach (ModuleConfigurationElement module in config.Modules)
                {
                    RegisterModule(module.ModuleName, module.Path, module.ModuleConfigurationFullName);
                }
            }
        }

        /// <summary>
        /// Checks if the module is loaded.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <returns>True if loaded</returns>
        public bool IsModuleLoaded(string moduleName)
        {
            return modules[moduleName].IsLoaded;
        }

        /// <summary>
        /// Registers a module to load on demand.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <param name="path">The path (relative or absolute)</param>
        /// <param name="moduleConfigurationFullName">The module configuration full name</param>
        public void RegisterModule(string moduleName, string path, string moduleConfigurationFullName)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (moduleConfigurationFullName == null)
                throw new ArgumentNullException(nameof(moduleConfigurationFullName));

            var module = new ModuleInfo(moduleName, path, moduleConfigurationFullName);
            modules[moduleName] = module;
        }

        /// <summary>
        /// Loads the module.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        public void LoadModule(string moduleName)
        {
            if (moduleName == null)
                throw new ArgumentNullException(nameof(moduleName));

            if (modules.TryGetValue(moduleName, out ModuleInfo module))
            {
                if (!module.IsLoaded)
                {
                    try
                    {
                        Assembly assembly = AssemblyLoader.LoadFile(module.Path);

                        ModuleInitializer.Initialize(module, assembly);

                        module.isLoaded = true;

                        OnModuleLoaded(module.ModuleName, assembly);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        private void OnModuleLoaded(string moduleName, Assembly assembly)
        {
            ModuleLoaded?.Invoke(this, new ModuleLoadedEventArgs(moduleName, assembly));
        }
    }
}
