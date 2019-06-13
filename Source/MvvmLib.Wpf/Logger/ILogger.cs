namespace MvvmLib.Logger
{
    /// <summary>
    /// The logger contract.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log the message.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="category">The category</param>
        /// <param name="priority">The priority</param>
        void Log(string message, Category category, Priority priority);
    }
}