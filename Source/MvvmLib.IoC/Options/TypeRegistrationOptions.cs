using System;

namespace MvvmLib.IoC
{

    public class TypeRegistrationOptions : RegistrationOptions
    {
        private TypeRegistration registration;

        public TypeRegistrationOptions(Injector injector, TypeRegistration registration)
            : base(injector)
        {
            this.registration = registration;
        }

        public TypeRegistrationOptions AsSingleton()
        {
            registration.IsSingleton = true;
            return this;
        }

        public TypeRegistrationOptions AsMultiInstances()
        {
            registration.IsSingleton = false;
            if(injector.IsCached(registration.TypeFrom, registration.Name))
            {
                injector.RemoveFromCache(registration.TypeFrom, registration.Name);
            }
            return this;
        }

        public TypeRegistrationOptions WithValueContainer(ValueContainer valueContainer)
        {
            registration.ValueContainer = valueContainer;
            return this;
        }

        public TypeRegistrationOptions OnResolved(Action<ContainerRegistration, object> callback)
        {
            registration.OnResolved = callback;
            return this;
        }
    }
}