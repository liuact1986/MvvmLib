using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// The options for type registration.
    /// </summary>
    public sealed class TypeRegistrationOptions
    {
        internal readonly TypeRegistration registration;
        private readonly Action<Type, string> clearCacheForType;

        internal TypeRegistrationOptions(TypeRegistration registration, Action<Type, string> clearCacheForType)
        {
            this.registration = registration;
            this.clearCacheForType = clearCacheForType;
        }

        /// <summary>
        /// Allows to get always the same instance for the registration.
        /// </summary>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions AsSingleton()
        {
            registration.IsSingleton = true;
            return this;
        }

        /// <summary>
        /// Allows to get always a new instance for the registration.
        /// </summary>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions AsMultiInstances()
        {
            registration.IsSingleton = false;
            clearCacheForType(registration.TypeTo, registration.Name);
            return this;
        }

        /// <summary>
        /// Value container for injected value types or collections.
        /// </summary>
        /// <param name="valueContainer">The value container dictionary</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions WithValueContainer(ValueContainer valueContainer)
        {
            registration.ValueContainer = valueContainer;
            return this;
        }

        /// <summary>
        /// Sets the resolved action for the registration.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The registration options</returns>
        public TypeRegistrationOptions OnResolved(Action<ContainerRegistration, object> action)
        {
            registration.onResolved = action;
            return this;
        }
    }
}
