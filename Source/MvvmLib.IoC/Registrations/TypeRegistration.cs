using System;

namespace MvvmLib.IoC
{

    /// <summary>
    /// The type registration class.
    /// </summary>
    public sealed class TypeRegistration : ContainerRegistration
    {
        private readonly Type typeFrom;
        /// <summary>
        /// The type from.
        /// </summary>
        public Type TypeFrom
        {
            get { return typeFrom; }
        }

        private readonly Type typeTo;
        /// <summary>
        /// The implementation type.
        /// </summary>
        public Type TypeTo
        {
            get { return typeTo; }
        }

        private bool isSingleton;
        /// <summary>
        /// Allows to get always the same instance.
        /// </summary>
        public bool IsSingleton
        {
            get { return isSingleton; }
            internal set { isSingleton = value; }
        }

        private ValueContainer valueContainer;
        /// <summary>
        /// The container for injected value types or collections.
        /// </summary>
        public ValueContainer ValueContainer
        {
            get { return valueContainer; }
            internal set { valueContainer = value; }
        }

        /// <summary>
        /// Creates the type registration class.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <param name="name">the name / key</param>
        /// <param name="typeTo">The implementation type</param>
        public TypeRegistration(Type typeFrom, string name, Type typeTo)
        {
            this.typeFrom = typeFrom;
            this.name = name;
            this.typeTo = typeTo;
            this.ValueContainer = new ValueContainer();
        }

    }
}
