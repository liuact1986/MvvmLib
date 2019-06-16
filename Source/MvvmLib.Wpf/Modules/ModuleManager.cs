using System.Collections.Generic;
using System.Configuration;

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
                    RegisterModule(module.Name, module.File, module.ModuleConfigFullName);
                }
            }
        }

        /// <summary>
        /// Registers a module to load on demand.
        /// </summary>
        /// <param name="name">The module name</param>
        /// <param name="file">The file</param>
        /// <param name="moduleConfigFullName">The module config full name</param>
        public static void RegisterModule(string name, string file, string moduleConfigFullName)
        {
            var module = new ModuleInfo(name, file, moduleConfigFullName);
            modules[name] = module;
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
                    var assembly = AssemblyLoader.Load(module.File);
                    if (assembly == null)
                        throw new ModuleLoadingFailException($"No assembly found for the file '{module.File}'");

                    AssemblyLoader.InitializeModule(assembly, module.ModuleConfigFullName);
                    module.isLoaded = true;
                }
            }
        }
    }

}
