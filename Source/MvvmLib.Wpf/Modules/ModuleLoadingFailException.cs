using System;
using System.Runtime.Serialization;

namespace MvvmLib.Modules
{
    [Serializable]
    internal class ModuleLoadingFailException : Exception
    {
        public ModuleLoadingFailException()
        {
        }

        public ModuleLoadingFailException(string message) : base(message)
        {
        }

        public ModuleLoadingFailException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ModuleLoadingFailException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}