using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation exception class.
    /// </summary>
    public class NavigationException : Exception
    {
        /// <summary>
        /// Creates the exception.
        /// </summary>
        public NavigationException()
        {
        }


        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">The message</param>
        public NavigationException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public NavigationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}