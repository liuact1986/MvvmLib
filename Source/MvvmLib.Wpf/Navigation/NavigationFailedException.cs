using System;
using System.Runtime.Serialization;

namespace MvvmLib.Navigation
{
    public enum NavigationFailedExceptionType
    {
        DeactivationCancelled,
        ActivationCancelled,
        ExceptionThrown
    }

    public enum NavigationFailedSourceType
    {
        View,
        Context,
        InnerException
    }

    [Serializable]
    public class NavigationFailedException : Exception
    {
        public NavigationFailedExceptionType NavivationFailedExceptionType { get; }

        public NavigationFailedSourceType NavigationFailedSourceType { get; }

        public object ViewOrContext { get; }

        public IRegion Region { get; }

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
            NavigationFailedSourceType navigationFailedSourceType, object viewOrContext, IRegion region)
        {
            this.NavivationFailedExceptionType = navigationFailedExceptionType;
            this.NavigationFailedSourceType = navigationFailedSourceType;
            this.ViewOrContext = viewOrContext;
            this.Region = region;
        }

        protected NavigationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public override string ToString()
        {
            return $"\"{NavivationFailedExceptionType}\" by the \"{NavigationFailedSourceType}\", Source \"{ViewOrContext}\", Region \"{Region.RegionName}\"";
        }
    }
}
