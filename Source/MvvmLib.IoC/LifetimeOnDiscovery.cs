namespace MvvmLib.IoC
{
    /// <summary>
    /// The lifetime with <see cref="Injector.AutoDiscovery"/>.
    /// </summary>
    public enum LifetimeOnDiscovery
    {
        /// <summary>
        /// New instance for all resolved types.
        /// </summary>
        Transcient,
        /// <summary>
        /// Singleton only for interfaces.
        /// </summary>
        SingletonOnlyForServices
    }
}
