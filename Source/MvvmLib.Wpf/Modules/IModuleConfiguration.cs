namespace MvvmLib.Modules
{
    /// <summary>
    /// Allows to manage a module loaded on demand.
    /// </summary>
    public interface IModuleConfiguration
    {
        /// <summary>
        /// Allows to register types for navigation and initialize the module.
        /// </summary>
        void Initialize();
    }
}
