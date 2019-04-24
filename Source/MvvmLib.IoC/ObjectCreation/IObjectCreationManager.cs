using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to manage instance creation with caches.
    /// </summary>
    public interface IObjectCreationManager
    {
        /// <summary>
        /// The factory cache.
        /// </summary>
        ConcurrentDictionary<Type, Func<object>> FactoryCache { get; }

        /// <summary>
        /// The parameterized factory cache.
        /// </summary>
        ConcurrentDictionary<Type, Func<object[], object>> ParameterizedFactoryCache { get; }

        /// <summary>
        /// The delegate factory.
        /// </summary>
        IDelegateFactory DelegateFactory { get; }

        /// <summary>
        /// The delegate factory type.
        /// </summary>
        DelegateFactoryType DelegateFactoryType { get; set; }

        /// <summary>
        /// Clears the caches and resets the delegate factory.
        /// </summary>
        void ClearCachesAndResetFactory();

        /// <summary>
        /// Creates an instance with an empty constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The instance created</returns>
         object CreateInstanceWithEmptyConstructor(Type type, ConstructorInfo constructor);

        /// <summary>
        /// Creates an instance with a parameterized constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="constructor">The constructor info</param>
        /// <param name="parameters">The parameters to inject</param>
        /// <returns>The instance created</returns>
        object CreateInstanceWithParameterizedConstructor(Type type, ConstructorInfo constructor, object[] parameters);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void ClearCaches();
    }
}
