using MvvmLib.IoC.Registrations;
using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Container for type registration options list.
    /// </summary>
    public class TypeRegistrationOptionsContainer
    {
        private readonly List<TypeRegistrationOptions> typeRegistrationOptionsList;

        /// <summary>
        /// Gets the registration options by the type from.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <returns>The type registration options found or null</returns>
        public TypeRegistrationOptions this[Type typeFrom]
        {
            get
            {
                foreach (var typeRegistrationOptions in typeRegistrationOptionsList)
                {
                    if (typeRegistrationOptions.registration.TypeFrom == typeFrom)
                        return typeRegistrationOptions;
                }
                return null;
            }
        }

        internal TypeRegistrationOptionsContainer(List<TypeRegistrationOptions> typeRegistrationOptionsList)
        {
            this.typeRegistrationOptionsList = typeRegistrationOptionsList ?? throw new ArgumentNullException(nameof(typeRegistrationOptionsList));
        }

        /// <summary>
        /// Allows to get always the same instance for the registrations.
        /// </summary>
        /// <returns>The registration options container</returns>
        public TypeRegistrationOptionsContainer AsSingleton()
        {
            foreach (var registrationOptions in typeRegistrationOptionsList)
                registrationOptions.AsSingleton();
            return this;
        }

        /// <summary>
        /// Allows to get always a new instance for the registrations.
        /// </summary>
        /// <returns>The registration options container</returns>
        public TypeRegistrationOptionsContainer AsMultiInstances()
        {
            foreach (var registrationOptions in typeRegistrationOptionsList)
                registrationOptions.AsMultiInstances();
            return this;
        }

        /// <summary>
        /// Sets the resolved action for the registrations.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The registration options container</returns>
        public TypeRegistrationOptionsContainer OnResolved(Action<ContainerRegistration, object> action)
        {
            foreach (var registrationOptions in typeRegistrationOptionsList)
                registrationOptions.OnResolved(action);

            return this;
        }
    }
}
