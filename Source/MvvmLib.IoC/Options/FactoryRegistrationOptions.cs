using System;

namespace MvvmLib.IoC
{
    public class FactoryRegistrationOptions : RegistrationOptions
    {
        private FactoryRegistration registration;

        public FactoryRegistrationOptions(Injector injector, FactoryRegistration registration)
            :base(injector)
        {
            this.registration = registration;
        }

        public FactoryRegistrationOptions OnResolved(Action<ContainerRegistration, object> callback)
        {
            registration.OnResolved = callback;
            return this;
        }
    }
}