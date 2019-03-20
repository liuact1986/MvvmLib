using System;
using System.Reflection;

namespace MvvmLib.IoC
{
    public class ReflectionDelegateFactory : IDelegateFactory
    {
        public Func<T> CreateConstructor<T>(Type type, ConstructorInfo constructor)
        {
            return () => (T)constructor.Invoke(null);
        }

        public Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo ctor)
        {
            return (p) => (T)ctor.Invoke(p);
        }
    }
}
