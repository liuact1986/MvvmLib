using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    public interface IInjectorResolver
    {
        bool AutoDiscovery { get; set; }
        bool NonPublicConstructors { get; set; }

        DelegateFactoryType DelegateFactoryType { get; set; }
        event EventHandler<InjectorResolveEventArgs> Resolved;

        object BuildUp(object instance);
        object BuildUp(Type type, string name);
        object BuildUp(Type type);
        void Clear();
        void ClearCache();
        List<object> GetAllInstances(Type type);
        object GetFromCache(Type type, string name);
        object GetFromCache(Type type);
        object GetInstance(Type type);
        object GetInstance(Type type, string name);
        object GetNewInstance(Type type);
        object GetNewInstance(Type type, string name);
        bool IsCached(Type type);
        bool IsCached(Type type, string name);
    }
}