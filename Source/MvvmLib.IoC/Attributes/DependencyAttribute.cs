using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to register a property as dependency for BuildUp.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DependencyAttribute : Attribute
    {
        private string name;
        /// <summary>
        /// The name/ key used for the registration.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}
