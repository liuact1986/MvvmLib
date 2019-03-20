using System;

namespace MvvmLib.IoC
{
    public class InstanceRegistration : ContainerRegistration
    {
        public Type Type { get; }

        public object Instance { get; }

        public InstanceRegistration(Type type, object instance, string name)
            : base(ContainerRegistrationType.Instance, name)
        {
            this.Type = type;
            this.Instance = instance;
        }
    }
}