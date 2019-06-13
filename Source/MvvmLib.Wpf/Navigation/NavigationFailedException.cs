using System;
using System.Runtime.Serialization;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Exception raised on navigation cancelled.
    /// </summary>
    [Serializable]
    public class NavigationFailedException : Exception
    {
        private readonly object originalSource;
        /// <summary>
        /// The source (View or ViewModel) .
        /// </summary>
        public object OriginalSource
        {
            get { return originalSource; }
        }

        private readonly object sender;
        /// <summary>
        /// The sender (NavigationSource or SharedSource).
        /// </summary>
        public object Sender
        {
            get { return sender; }
        }

        /// <summary>
        /// Creates the <see cref="NavigationFailedException"/>.
        /// </summary>
        public NavigationFailedException()
        {
        }

        /// <summary>
        /// Creates the <see cref="NavigationFailedException"/>.
        /// </summary>
        /// <param name="message">The message</param>
        public NavigationFailedException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates the <see cref="NavigationFailedException"/>.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="innerException">The inner exception</param>
        public NavigationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates the <see cref="NavigationFailedException"/>.
        /// </summary>
        /// <param name="info">The info</param>
        /// <param name="context">The context</param>
        protected NavigationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Creates the <see cref="NavigationFailedException"/>.
        /// </summary>
        /// <param name="originalSource">The source (View or ViewModel)</param>
        /// <param name="sender">The sender (NavigationSource or SharedSource)</param>
        public NavigationFailedException(object originalSource, object sender)
        {
            this.originalSource = originalSource;
            this.sender = sender;
        }
    }
}