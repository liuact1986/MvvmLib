using System;

namespace MvvmLib.IoC
{
    public class FactoryRegistration : ContainerRegistration
    {
        public Type Type{ get; }

        public Func<object> Factory { get; }

        public FactoryRegistration(Type type, Func<object> factory, string name)
            : base(ContainerRegistrationType.Factory, name)
        {
            this.Type = type;
            this.Factory = factory;
        }
    }
}