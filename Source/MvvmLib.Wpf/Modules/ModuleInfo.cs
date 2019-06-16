using System;

namespace MvvmLib.Modules
{
    /// <summary>
    /// The module info.
    /// </summary>
    public class ModuleInfo
    {
        private readonly string name;
        /// <summary>
        /// The module name.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        private readonly string file;
        /// <summary>
        /// The file.
        /// </summary>
        public string File
        {
            get { return file; }
        }

        private readonly string moduleConfigFullName;
        /// <summary>
        /// The module config full name.
        /// </summary>
        public string ModuleConfigFullName
        {
            get { return moduleConfigFullName; }
        }

        internal bool isLoaded;
        /// <summary>
        /// Checks if is already loaded.
        /// </summary>
        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        internal ModuleInfo(string name, string file, string moduleConfigFullName)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (file == null)
                throw new ArgumentNullException(nameof(file));
            if (moduleConfigFullName == null)
                throw new ArgumentNullException(nameof(moduleConfigFullName));

            this.name = name;
            this.file = file;
            this.moduleConfigFullName = moduleConfigFullName;
        }
    }

}
