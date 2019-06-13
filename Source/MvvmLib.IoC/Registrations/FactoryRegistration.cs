using System;

namespace MvvmLib.IoC.Registrations
{
    /// <summary>
    /// The factory class.
    /// </summary>
    public sealed class FactoryRegistration : ContainerRegistration
    {
        private readonly Type type;
        /// <summary>
        /// The type.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        private readonly Func<object> factory;
        /// <summary>
        /// The factory.
        /// </summary>
        public Func<object> Factory
        {
            get { return factory; }
        }

        /// <summary>
        /// Creates the factory registration class.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="factory">The factory</param>
        public FactoryRegistration(Type type, string name, Func<object> factory)
        {
            this.type = type;
            this.name = name;
            this.factory = factory;
        }
    }
}
