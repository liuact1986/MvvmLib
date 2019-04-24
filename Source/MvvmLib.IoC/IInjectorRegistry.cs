using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to register types, factories, instances.
    /// </summary>
    public interface IInjectorRegistry
    {
        /// <summary>
        /// Invoked on registration.
        /// </summary>
        event EventHandler<RegistrationEventArgs> Registered;

        /// <summary>
        /// Checks if ther is a registration for the type with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>true if found</returns>
         bool IsRegistered(Type type, string name);

        /// <summary>
        /// Checks if ther is a registration for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>true if found</returns>
        bool IsRegistered(Type type);

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <param name="name">The name / key</param>
        /// <param name="typeTo">The implementation type</param>
        /// <returns>The registration options</returns>
         TypeRegistrationOptions RegisterType(Type typeFrom, string name, Type typeTo);

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="typeFrom">The type from</param>
        /// <param name="typeTo">The implementation type</param>
        /// <returns>The registration options</returns>
         TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo);
        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>The registration options</returns>
         TypeRegistrationOptions RegisterType(Type type);

        /// <summary>
        /// registers a type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
         TypeRegistrationOptions RegisterType(Type type, string name);

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
       InstanceRegistrationOptions RegisterInstance(Type type, string name, object instance);

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
        InstanceRegistrationOptions RegisterInstance(Type type, object instance);

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        FactoryRegistrationOptions RegisterFactory(Type type, string name, Func<object> factory);

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory);

        /// <summary>
        /// Unregisters all registrations for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if unregistered</returns>
         bool UnregisterAll(Type type);

        /// <summary>
        /// Unregisters the registration for the type with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>True if unregistered</returns>
        bool Unregister(Type type, string name);

        /// <summary>
        /// Unregisters the registration for the type.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if unregistered</returns>
        bool Unregister(Type type);
    }
}