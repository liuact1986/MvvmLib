using System;

namespace MvvmLib.IoC.Registrations
{
    /// <summary>
    /// The instance registration class.
    /// </summary>
    public sealed class InstanceRegistration : ContainerRegistration
    {
        private readonly Type type;
        /// <summary>
        /// The type.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        private object instance;
        /// <summary>
        /// The instance.
        /// </summary>
        public object Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Creates the instance registration class.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        public InstanceRegistration(Type type, string name, object instance)
        {
            this.type = type;
            this.name = name;
            this.instance = instance;
        }
    }
}
