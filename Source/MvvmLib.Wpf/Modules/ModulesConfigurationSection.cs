using System.Configuration;

namespace MvvmLib.Modules
{
    /// <summary>
    /// Custom Configuration section for "modules" registration.
    /// </summary>
    public class ModulesConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// The Modules section.
        /// </summary>
        [ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        public ModuleConfigurationElementCollection Modules
        {
            get { return (ModuleConfigurationElementCollection)base[""]; }
            set { base[""] = value; }
        }

        //[ConfigurationProperty("", IsDefaultCollection = true, IsKey = false)]
        //public ModuleConfigurationElementCollection Modules
        //{
        //    get { return (ModuleConfigurationElementCollection)base["modules"]; }
        //    set { base["modules"] = value; }
        //}
    }

}
