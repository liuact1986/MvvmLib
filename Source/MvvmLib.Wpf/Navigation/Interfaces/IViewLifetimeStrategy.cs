namespace MvvmLib.Navigation
{
    /// <summary>
    /// Strategy type.
    /// </summary>
    public enum StrategyType
    {
        /// <summary>
        /// Always the same instance.
        /// </summary>
        Singleton,
        /// <summary>
        /// Always a new instance.
        /// </summary>
        Transcient
    }

    /// <summary>
    /// Allows to get always the same instance of view (Singleton) or always a new instance (Transcient).
    /// </summary>
    public interface IViewLifetimeStrategy
    {
        /// <summary>
        /// Gets the strategy.
        /// </summary>
        StrategyType Strategy { get; }
    }
}
