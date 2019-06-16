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
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// The file (location of .dll).
        /// </summary>
        [ConfigurationProperty("file", IsRequired = true)]
        public string File
        {
            get { return (string)this["file"]; }
            set { base["file"] = value; }
        }

        /// <summary>
        /// The module config full name (namespace + config name)
        /// </summary>
        [ConfigurationProperty("moduleConfigFullName", IsRequired = true)]
        public string ModuleConfigFullName
        {
            get { return (string)this["moduleConfigFullName"]; }
            set { base["moduleConfigFullName"] = value; }
        }
    }

}
