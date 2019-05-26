using System;
using System.Runtime.Serialization;

namespace MvvmLib.Navigation
{
    [Serializable]
    public class NavigationFailedException : Exception
    {
        private readonly NavigationFailedExceptionType navigationFailedExceptionType;
        public NavigationFailedExceptionType NavigationFailedExceptionType
        {
            get { return navigationFailedExceptionType; }
        }

        private readonly NavigationFailedSourceType navigationFailedSourceType;
        public NavigationFailedSourceType NavigationFailedSourceType
        {
            get { return navigationFailedSourceType; }
        }

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

        public NavigationFailedException(NavigationFailedExceptionType navigationFailedExceptionType,
            NavigationFailedSourceType navigationFailedSourceType, object originalSource, object sender)
        {
            this.navigationFailedExceptionType = navigationFailedExceptionType;
            this.navigationFailedSourceType = navigationFailedSourceType;
            this.originalSource = originalSource;
            this.sender = sender;
        }

        protected NavigationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return $"\"{navigationFailedExceptionType}\" by the \"{navigationFailedSourceType}\", Source \"{originalSource}\", Sender \"{sender}\"";
        }
    }

    public enum NavigationFailedExceptionType
    {
        DeactivationCancelled,
        ActivationCancelled,
        ExceptionThrown
    }

    public enum NavigationFailedSourceType
    {
        Source,
        Context,
        InnerException
    }
}