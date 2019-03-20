using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MvvmLib.IoC
{
    public class ObjectCreationManager : IObjectCreationManager
    {
        private readonly ConcurrentDictionary<Type, Func<object>> factoryCache
            = new ConcurrentDictionary<Type, Func<object>>();

        private readonly ConcurrentDictionary<Type, Func<object[], object>> parameterizedFactoryCache
            = new ConcurrentDictionary<Type, Func<object[], object>>();

        private IDelegateFactory delegateFactory;

        private DelegateFactoryType delegateFactoryType;
        public DelegateFactoryType DelegateFactoryType
        {
            get { return delegateFactoryType; }
            set
            {
                if (value != delegateFactoryType)
                {
                    delegateFactoryType = value;
                    this.ClearCache();
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

        public ObjectCreationManager()
        {
            Initialize();
        }

        internal void Initialize()
        {
            delegateFactoryType = DelegateFactoryType.Expression;
            delegateFactory = new ExpressionDelegateFactory();
        }

        public void Reset()
        {
            this.ClearCache();
            Initialize();
        }

        public bool TryGetConstructorFromCache(Type type, out Func<object> factory)
        {
            return factoryCache.TryGetValue(type, out factory);
        }

        public bool TryGetConstructorFromCache(Type type, out Func<object[], object> factory)
        {
            return parameterizedFactoryCache.TryGetValue(type, out factory);
        }

        public object CreateInstance(Type type, ConstructorInfo constructor)
        {
            if (TryGetConstructorFromCache(type, out Func<object> cached))
            {
                return cached();
            }
            else
            {
                var factory = delegateFactory.CreateConstructor<object>(type, constructor);
                factoryCache[type] = factory;
                return factory();
            }
        }

        public object CreateInstance(Type type, ConstructorInfo constructor, object[] parameters)
        {
            if (TryGetConstructorFromCache(type, out Func<object[], object> cached))
            {
                return cached(parameters);
            }
            else
            {
                var factory = delegateFactory.CreateParameterizedConstructor<object>(type, constructor);
                parameterizedFactoryCache[type] = factory;
                return factory(parameters);
            }
        }

        public void ClearCache()
        {
            factoryCache.Clear();
            parameterizedFactoryCache.Clear();
        }
    }
}
