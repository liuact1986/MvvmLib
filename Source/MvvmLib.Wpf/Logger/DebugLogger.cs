using System;
using System.Diagnostics;
using System.Globalization;

namespace MvvmLib.Logger
{
    /// <summary>
    /// The default Logger. Allows to write message with <see cref="Debug"/>.
    /// </summary>
    public class DebugLogger : ILogger
    {
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="category">The category</param>
        /// <param name="priority">The priority</param>
        public void Log(string message, Category category, Priority priority)
        {
            string messageToLog = string.Format(CultureInfo.InvariantCulture, "{1}: {2}. Priority: {3}. Timestamp:{0:u}.", DateTime.Now, category.ToString().ToUpper(), message, priority);

            Debug.WriteLine(messageToLog);
        }
    }
}
