using MvvmLib.IoC.Registrations;
using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// The options for instance registration.
    /// </summary>
    public sealed class InstanceRegistrationOptions
    {
        private readonly InstanceRegistration registration;

        internal InstanceRegistrationOptions(InstanceRegistration registration)
        {
            this.registration = registration;
        }

        /// <summary>
        /// Sets the resolved action for the registration.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The registration options</returns>
        public InstanceRegistrationOptions OnResolved(Action<ContainerRegistration, object> action)
        {
            registration.onResolved = action;
            return this;
        }
    }
}
