using System;
using System.Reflection;

namespace MvvmLib.IoC
{
    public interface IDelegateFactory
    {
        Func<T> CreateConstructor<T>(Type type, ConstructorInfo constructor);
        Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo ctor);
    }
}