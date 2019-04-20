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
        ViewOrObject,
        Context,
        InnerException
    }

    [Serializable]
    public class NavigationFailedException : Exception
    {
        private readonly NavigationFailedExceptionType navivationFailedExceptionType;
        public NavigationFailedExceptionType NavivationFailedExceptionType
        {
            get { return navivationFailedExceptionType; }
        }

        private readonly NavigationFailedSourceType navigationFailedSourceType;
        public NavigationFailedSourceType NavigationFailedSourceType
        {
            get { return navigationFailedSourceType; }
        }

        private readonly object viewOrContext;
        public object ViewOrContext
        {
            get { return viewOrContext; }
        }

        private readonly IRegion region;
        public IRegion Region
        {
            get { return region; }
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
            NavigationFailedSourceType navigationFailedSourceType, object viewOrContext, IRegion region)
        {
            this.navivationFailedExceptionType = navigationFailedExceptionType;
            this.navigationFailedSourceType = navigationFailedSourceType;
            this.viewOrContext = viewOrContext;
            this.region = region;
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
