using System;
using System.Runtime.Serialization;

namespace MvvmLib.IoC
{
    [Serializable]
    internal class RegistrationFailedException : Exception
    {
        public RegistrationFailedException()
        {
        }

        public RegistrationFailedException(string message) : base(message)
        {
        }

        public RegistrationFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RegistrationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}