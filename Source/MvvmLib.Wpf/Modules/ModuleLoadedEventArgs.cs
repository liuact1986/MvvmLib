using System.Reflection;

namespace MvvmLib.Modules
{
    /// <summary>
    /// The module loaded event args class.
    /// </summary>
    public class ModuleLoadedEventArgs
    {
        private readonly string moduleName;
        /// <summary>
        /// The module name.
        /// </summary>
        public string ModuleName
        {
            get { return moduleName; }
        }

        private readonly Assembly assembly;
        /// <summary>
        /// The assembly.
        /// </summary>
        public Assembly Assembly
        {
            get { return assembly; }
        }

        /// <summary>
        /// Creates the <see cref="ModuleLoadedEventArgs"/>.
        /// </summary>
        /// <param name="moduleName">The module name</param>
        /// <param name="assembly">The assembly</param>
        public ModuleLoadedEventArgs(string moduleName, Assembly assembly)
        {
            this.moduleName = moduleName;
            this.assembly = assembly;
        }
    }
}
