using System;

namespace MvvmLib.IoC
{
    public interface IInjectorRegistry
    {
        event EventHandler<InjectorRegistrationEventArgs> Registered;

        bool IsRegistered(Type type);
        bool IsRegistered(Type type, string name);
        FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory);
        FactoryRegistrationOptions RegisterFactory(Type type, Func<object> factory, string name);
        InstanceRegistrationOptions RegisterInstance(Type type, object instance);
        InstanceRegistrationOptions RegisterInstance(Type type, object instance, string name);
        TypeRegistrationOptions RegisterType(Type type);
        TypeRegistrationOptions RegisterType(Type type, string name);
        TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo);
        TypeRegistrationOptions RegisterType(Type typeFrom, Type typeTo, string name);
        bool Unregister(Type type);
        bool Unregister(Type type, string name);
        bool UnregisterAll(Type type);
    }
}