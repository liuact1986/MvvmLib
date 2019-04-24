using System;
using System.Runtime.Serialization;

namespace MvvmLib.IoC
{
    [Serializable]
    internal class ResolutionFailedException : Exception
    {
        public ResolutionFailedException()
        {
        }

        public ResolutionFailedException(string message) : base(message)
        {
        }

        public ResolutionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ResolutionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}