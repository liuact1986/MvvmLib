using System;

namespace MvvmLib.IoC
{
    public static class IInjectorRegistryExtensions
    {

        public static FactoryRegistrationOptions RegisterFactory<T>(this IInjectorRegistry container, Func<object> factory)
        {
            return container.RegisterFactory(typeof(T), factory);
        }

        public static FactoryRegistrationOptions RegisterFactory<T>(this IInjectorRegistry container, Func<object> factory, string name)
        {
            return container.RegisterFactory(typeof(T), factory, name);
        }

        public static InstanceRegistrationOptions RegisterInstance<T>(this IInjectorRegistry container, object instance)
        {
            return container.RegisterInstance(typeof(T), instance);
        }

        public static InstanceRegistrationOptions RegisterInstance<T>(this IInjectorRegistry container, object instance, string name)
        {
            return container.RegisterInstance(typeof(T), instance, name);
        }

        public static TypeRegistrationOptions RegisterType<T>(this IInjectorRegistry container)
        {
            return container.RegisterType(typeof(T));
        }

        public static TypeRegistrationOptions RegisterType<T>(this IInjectorRegistry container, string name)
        {
            return container.RegisterType(typeof(T), name);
        }

        public static TypeRegistrationOptions RegisterType<TFrom, TTo>(this IInjectorRegistry container) 
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo));
        }

        public static TypeRegistrationOptions RegisterType<TFrom, TTo>(this IInjectorRegistry container, string name) 
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo), name);
        }

        public static TypeRegistrationOptions RegisterSingleton<T>(this IInjectorRegistry container)
        {
            return container.RegisterType(typeof(T)).AsSingleton();
        }

        public static TypeRegistrationOptions RegisterSingleton<T>(this IInjectorRegistry container, string name)
        {
            return container.RegisterType(typeof(T), name).AsSingleton();
        }

        public static TypeRegistrationOptions RegisterSingleton<TFrom, TTo>(this IInjectorRegistry container) 
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo)).AsSingleton();
        }

        public static TypeRegistrationOptions RegisterSingleton<TFrom, TTo>(this IInjectorRegistry container,string name) 
            where TTo : TFrom
        {
            return container.RegisterType(typeof(TFrom), typeof(TTo), name).AsSingleton();
        }

        public static bool IsRegistered<T>(this IInjectorRegistry container, string name)
        {
            return container.IsRegistered(typeof(T), name);
        }

        public static bool IsRegistered<T>(this IInjectorRegistry container)
        {
            return container.IsRegistered(typeof(T));
        }

        public static bool UnregisterAll<T>(this IInjectorRegistry container)
        {
            return container.UnregisterAll(typeof(T));
        }

        public static bool Unregister<T>(this IInjectorRegistry container)
        {
            return container.Unregister(typeof(T));
        }

        public static bool Unregister<T>(this IInjectorRegistry container, string name)
        {
            return container.Unregister(typeof(T), name);
        }
    }
}