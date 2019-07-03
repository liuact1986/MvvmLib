using MvvmLib.Navigation;
using System;
using System.Reflection;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to initialize modules.
    /// </summary>
    public class ModuleInitializer
    {
        /// <summary>
        /// Creates the <see cref="IModuleConfiguration"/> instance and invokes <see cref="IModuleConfiguration.Initialize"/>.
        /// </summary>
        /// <param name="module">The module</param>
        /// <param name="assembly">The assembly</param>
        public static void Initialize(ModuleInfo module, Assembly assembly)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var moduleConfigurationType = assembly.GetType(module.ModuleConfigurationFullName);
            if (moduleConfigurationType == null)
                throw new ArgumentException($"Unable to resolve the type for '{module.ModuleConfigurationFullName}'");

            var instance = SourceResolver.CreateInstance(moduleConfigurationType);
            var moduleConfiguration = instance as IModuleConfiguration;
            if (moduleConfiguration == null)
                throw new ArgumentException($"The module configuration '{module.ModuleConfigurationFullName}' ('{module.ModuleName}') has to implement IModuleConfiguration interface");

            moduleConfiguration.Initialize();
        }
    }
}
