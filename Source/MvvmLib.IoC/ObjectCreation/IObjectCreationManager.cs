using System;
using System.Reflection;

namespace MvvmLib.IoC
{
    public interface IObjectCreationManager
    {
        DelegateFactoryType DelegateFactoryType { get; set; }

        void ClearCache();
        object CreateInstance(Type type, ConstructorInfo constructor);
        object CreateInstance(Type type, ConstructorInfo constructor, object[] parameters);
        void Reset();
        bool TryGetConstructorFromCache(Type type, out Func<object[], object> factory);
        bool TryGetConstructorFromCache(Type type, out Func<object> factory);
    }
}