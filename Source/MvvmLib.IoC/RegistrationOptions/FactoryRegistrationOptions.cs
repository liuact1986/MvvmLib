using System;

namespace MvvmLib.IoC
{

    /// <summary>
    /// The options for factory registration.
    /// </summary>
    public sealed class FactoryRegistrationOptions
    {
        private readonly FactoryRegistration registration;

        internal FactoryRegistrationOptions(FactoryRegistration registration)
        {
            this.registration = registration;
        }

        /// <summary>
        /// Sets the resolved action for the registration.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The registration options</returns>
        public FactoryRegistrationOptions OnResolved(Action<ContainerRegistration, object> action)
        {
            registration.onResolved = action;
            return this;
        }
    }
}
