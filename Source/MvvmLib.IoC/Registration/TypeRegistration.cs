using System;

namespace MvvmLib.IoC
{
    public class TypeRegistration : ContainerRegistration
    {
        public Type TypeFrom { get; }

        public Type TypeTo { get; }

        public bool IsSingleton { get; internal set; }

        public ValueContainer ValueContainer { get; internal set; }

        public TypeRegistration(Type typeFrom, Type typeTo, string name)
            : base(ContainerRegistrationType.Type, name)
        {
            this.TypeFrom = typeFrom;
            this.TypeTo = typeTo;
            this.ValueContainer = new ValueContainer();
        }

        public bool HasValue(string key)
        {
            return this.ValueContainer.IsRegistered(key);
        }

        public object GetValue(string key)
        {
            return this.ValueContainer.Get(key);
        }
    }

}