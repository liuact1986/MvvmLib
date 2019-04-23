namespace MvvmLib.Message
{
    /// <summary>
    /// The execution strategy.
    /// </summary>
    public enum ExecutionStrategyType
    {
        /// <summary>
        /// Publish Thread (default)
        /// </summary>
        PublisherThread,
        /// <summary>
        ///  UI Thread
        /// </summary>
        UIThread,
        /// <summary>
        /// Background Thread
        /// </summary>
        BackgroundThread
    }
}
