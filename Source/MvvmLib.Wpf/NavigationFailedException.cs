using System;
using System.Runtime.Serialization;

namespace MvvmLib.Navigation
{
    [Serializable]
    public class NavigationFailedException : Exception
    {
        private readonly object originalSource;
        public object OriginalSource
        {
            get { return originalSource; }
        }

        private readonly object sender;
        public object Sender
        {
            get { return sender; }
        }

        public NavigationFailedException()
        {
        }

        public NavigationFailedException(string message) : base(message)
        {
        }

        public NavigationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NavigationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NavigationFailedException(object originalSource, object sender)
        {
            this.originalSource = originalSource;
            this.sender = sender;
        }
    }
}