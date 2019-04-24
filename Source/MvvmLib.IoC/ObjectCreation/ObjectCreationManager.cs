using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MvvmLib.IoC
{

    /// <summary>
    /// Allows to manage instance creation with caches.
    /// </summary>
    public sealed class ObjectCreationManager : IObjectCreationManager
    {
        private readonly ConcurrentDictionary<Type, Func<object>> factoryCache;

        /// <summary>
        /// The factory cache.
        /// </summary>
        public ConcurrentDictionary<Type, Func<object>> FactoryCache
        {
            get { return factoryCache; }
        }

        private readonly ConcurrentDictionary<Type, Func<object[], object>> parameterizedFactoryCache;
        /// <summary>
        /// The parameterized factory cache.
        /// </summary>
        public ConcurrentDictionary<Type, Func<object[], object>> ParameterizedFactoryCache
        {
            get { return parameterizedFactoryCache; }
        }


        private IDelegateFactory delegateFactory;
        /// <summary>
        /// The delegate factory.
        /// </summary>
        public IDelegateFactory DelegateFactory
        {
            get { return delegateFactory; }
        }


        private DelegateFactoryType delegateFactoryType;
        /// <summary>
        /// The delegate factory type.
        /// </summary>
        public DelegateFactoryType DelegateFactoryType
        {
            get { return delegateFactoryType; }
            set
            {
                if (value != delegateFactoryType)
                {
                    delegateFactoryType = value;
                    this.ClearCaches();
                    switch (value)
                    {
                        case DelegateFactoryType.Reflection:
                            delegateFactory = new ReflectionDelegateFactory();
                            break;
                        case DelegateFactoryType.Expression:
                            delegateFactory = new ExpressionDelegateFactory();
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// Creates the object creation manager class.
        /// </summary>
        public ObjectCreationManager()
        {
            factoryCache = new ConcurrentDictionary<Type, Func<object>>();
            parameterizedFactoryCache = new ConcurrentDictionary<Type, Func<object[], object>>();
            Initialize();
        }

        internal void Initialize()
        {
            delegateFactoryType = DelegateFactoryType.Expression;
            delegateFactory = new ExpressionDelegateFactory();
        }

        /// <summary>
        /// Clears the caches and resets the delegate factory.
        /// </summary>
        public void ClearCachesAndResetFactory()
        {
            this.ClearCaches();
            Initialize();
        }

        /// <summary>
        /// Creates an instance with an empty constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The instance created</returns>
        public object CreateInstanceWithEmptyConstructor(Type type, ConstructorInfo constructor)
        {
            if (this.FactoryCache.TryGetValue(type, out Func<object> factory))
                return factory();
            else
            {
                factory = DelegateFactory.CreateConstructor<object>(type, constructor);
                FactoryCache[type] = factory;
                return factory();
            }
        }

        /// <summary>
        /// Creates an instance with a parameterized constructor.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="constructor">The constructor info</param>
        /// <param name="parameters">The parameters to inject</param>
        /// <returns>The instance created</returns>
        public object CreateInstanceWithParameterizedConstructor(Type type, ConstructorInfo constructor, object[] parameters)
        {
            if (this.ParameterizedFactoryCache.TryGetValue(type, out Func<object[], object> factory))
                return factory(parameters);
            else
            {
                factory = DelegateFactory.CreateParameterizedConstructor<object>(type, constructor);
                ParameterizedFactoryCache[type] = factory;
                return factory(parameters);
            }
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCaches()
        {
            FactoryCache.Clear();
            ParameterizedFactoryCache.Clear();
        }
    }
}
