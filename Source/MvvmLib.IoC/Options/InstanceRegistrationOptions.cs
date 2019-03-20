using System;

namespace MvvmLib.IoC
{
    public class InstanceRegistrationOptions : RegistrationOptions
    {
        private InstanceRegistration registration;

        public InstanceRegistrationOptions(Injector injector, InstanceRegistration registration)
            : base(injector)
        {
            this.registration = registration;
        }

        public InstanceRegistrationOptions OnResolved(Action<ContainerRegistration, object> callback)
        {
            registration.OnResolved = callback;
            return this;
        }
    }
}