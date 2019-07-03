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
        /// The module configuration full name (namespace + config name).
        /// </summary>
        [ConfigurationProperty("moduleConfigurationFullName", IsRequired = true)]
        public string ModuleConfigurationFullName
        {
            get { return (string)this["moduleConfigurationFullName"]; }
            set { base["moduleConfigurationFullName"] = value; }
        }
    }

}
