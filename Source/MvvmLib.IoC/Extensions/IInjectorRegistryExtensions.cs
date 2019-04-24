using System;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Injector registration extension methods.
    /// </summary>
    public static class IInjectorRegistryExtensions
    {
        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        public static FactoryRegistrationOptions RegisterFactory<T>(this IInjectorRegistry container, Func<object> factory)
        {
            return container.RegisterFactory(typeof(T), factory);
        }

        /// <summary>
        /// Registers a factory.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <param name="factory">The factory function</param>
        /// <returns>The registration options</returns>
        public static FactoryRegistrationOptions RegisterFactory<T>(this IInjectorRegistry container, string name, Func<object> factory)
        {
            return container.RegisterFactory(typeof(T), name, factory);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
        public static InstanceRegistrationOptions RegisterInstance<T>(this IInjectorRegistry container, object instance)
        {
            return container.RegisterInstance(typeof(T), instance);
        }

        /// <summary>
        /// Registers the instance.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The registration options</returns>
        public static InstanceRegistrationOptions RegisterInstance<T>(this IInjectorRegistry container, string name, object instance)
        {
            return container.RegisterInstance(typeof(T), name, instance);
        }

        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterType<T>(this IInjectorRegistry container)
        {
            return container.RegisterType(typeof(T));
        }

        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterType<T>(this IInjectorRegistry container, string name)
        {
            return container.RegisterType(typeof(T), name);
        }

        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <typeparam name="TFrom">the type from</typeparam>
        /// <typeparam name="TTo">The implementation type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterType<TFrom, TTo>(this IInjectorRegistry container)
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo));
        }

        /// <summary>
        /// Registers a type.
        /// </summary>
        /// <typeparam name="TFrom">the type from</typeparam>
        /// <typeparam name="TTo">The implementation type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterType<TFrom, TTo>(this IInjectorRegistry container, string name)
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), name, typeof(TTo));
        }

        /// <summary>
        /// Registers a type as singleton.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterSingleton<T>(this IInjectorRegistry container)
        {
            return container.RegisterType(typeof(T)).AsSingleton();
        }

        /// <summary>
        /// Registers a type as singleton.
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterSingleton<T>(this IInjectorRegistry container, string name)
        {
            return container.RegisterType(typeof(T), name).AsSingleton();
        }

        /// <summary>
        /// Registers a type as singleton.
        /// </summary>
        /// <typeparam name="TFrom">the type from</typeparam>
        /// <typeparam name="TTo">The implementation type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterSingleton<TFrom, TTo>(this IInjectorRegistry container)
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo)).AsSingleton();
        }

        /// <summary>
        /// Registers a type as singleton.
        /// </summary>
        /// <typeparam name="TFrom">the type from</typeparam>
        /// <typeparam name="TTo">The implementation type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>The registration options</returns>
        public static TypeRegistrationOptions RegisterSingleton<TFrom, TTo>(this IInjectorRegistry container, string name)
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), name, typeof(TTo)).AsSingleton();
        }

        /// <summary>
        /// Checks if the type with the name / key is registered.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>True if registered</returns>
        public static bool IsRegistered<T>(this IInjectorRegistry container, string name)
        {
            return container.IsRegistered(typeof(T), name);
        }

        /// <summary>
        /// Checks if the type is registered.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>True if registered</returns>
        public static bool IsRegistered<T>(this IInjectorRegistry container)
        {
            return container.IsRegistered(typeof(T));
        }

        /// <summary>
        /// Unregisters all registrations for the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>True if unregistered</returns>
        public static bool UnregisterAll<T>(this IInjectorRegistry container)
        {
            return container.UnregisterAll(typeof(T));
        }

        /// <summary>
        /// Unregisters the registration for the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>True if unregistered</returns>
        public static bool Unregister<T>(this IInjectorRegistry container)
        {
            return container.Unregister(typeof(T));
        }

        /// <summary>
        /// Unregisters the registration for the type with the name / key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name / key</param>
        /// <returns>True if unregistered</returns>
        public static bool Unregister<T>(this IInjectorRegistry container, string name)
        {
            return container.Unregister(typeof(T), name);
        }
    }
}