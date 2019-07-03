namespace MvvmLib.Modules
{
    /// <summary>
    /// The module info.
    /// </summary>
    public class ModuleInfo
    {
        private readonly string moduleName;
        /// <summary>
        /// The module name.
        /// </summary>
        public string ModuleName
        {
            get { return moduleName; }
        }

        private readonly string path;
        /// <summary>
        /// The path (relative or absolute).
        /// </summary>
        public string Path
        {
            get { return path; }
        }

        private readonly string moduleConfigurationFullName;
        /// <summary>
        /// The module configuration full name.
        /// </summary>
        public string ModuleConfigurationFullName
        {
            get { return moduleConfigurationFullName; }
        }

        internal bool isLoaded;
        /// <summary>
        /// Checks if is already loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        internal ModuleInfo(string moduleName, string path, string moduleConfigurationFullName)
        {
            this.moduleName = moduleName;
            this.path = path;
            this.moduleConfigurationFullName = moduleConfigurationFullName;
        }
    }
}
