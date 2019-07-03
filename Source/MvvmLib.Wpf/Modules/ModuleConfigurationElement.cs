using System.Configuration;

namespace MvvmLib.Modules
{
    /// <summary>
    /// The module configuration element.
    /// </summary>
    public class ModuleConfigurationElement : ConfigurationElement
    {
        /// <summary>
        /// The module name.
        /// </summary>
        [ConfigurationProperty("moduleName", IsRequired = true)]
        public string ModuleName
        {
            get { return this["moduleName"] as string; }
            set { base["moduleName"] = value; }
        }

        /// <summary>
        /// The file (location of .dll).
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true)]
        public string Path
        {
            get { return (string)this["path"]; }
            set { base["path"] = value; }
        }

        /// <summary>
        /// The module config full name (namespace + config name).
        /// </summary>
        [ConfigurationProperty("moduleConfigFullName", IsRequired = true)]
        public string ModuleConfigFullName
        {
            get { return (string)this["moduleConfigFullName"]; }
            set { base["moduleConfigFullName"] = value; }
        }
    }

}
