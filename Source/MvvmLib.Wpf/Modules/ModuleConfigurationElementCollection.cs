using System.Configuration;

namespace MvvmLib.Modules
{
    /// <summary>
    /// The Module configuration element collection.
    /// </summary>
    public class ModuleConfigurationElementCollection : ConfigurationElementCollection
    {

        /// <summary>
        /// Throws an exception on duplicate.
        /// </summary>
        protected override bool ThrowOnDuplicate
        {
            get { return true; }
        }

        /// <summary>
        /// The collection type.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// The element name ("module").
        /// </summary>
        protected override string ElementName
        {
            get { return "module"; }
        }

        /// <summary>
        /// Gets the configuration element at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The configuration element</returns>
        public ModuleConfigurationElement this[int index]
        {
            get { return (ModuleConfigurationElement)base.BaseGet(index); }
        }

        /// <summary>
        /// Adds a module configuration element.
        /// </summary>
        /// <param name="module"></param>
        public void Add(ModuleConfigurationElement module)
        {
            BaseAdd(module);
        }

        /// <summary>
        /// Checks if a module is already registered for the module name.
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public bool Contains(string moduleName)
        {
            return base.BaseGet(moduleName) != null;
        }

        /// <summary>
        /// Creates a configuration element.
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }

        /// <summary>
        /// Gets the configuration element key.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>The key</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement)element).Name;
        }
    }

}
