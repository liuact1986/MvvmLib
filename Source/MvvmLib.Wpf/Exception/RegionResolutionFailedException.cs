using System;
using System.Runtime.Serialization;

namespace MvvmLib.Navigation
{
    [Serializable]
    internal class RegionResolutionFailedException : Exception
    {
        public RegionResolutionFailedException()
        {
        }

        public RegionResolutionFailedException(string message) : base(message)
        {
        }

        public RegionResolutionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegionResolutionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}