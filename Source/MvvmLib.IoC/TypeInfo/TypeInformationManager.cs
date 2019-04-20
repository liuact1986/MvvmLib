using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.IoC
{
    public class TypeInformationManager
    {
        protected Dictionary<Type, TypeInformation> typesCache
           = new Dictionary<Type, TypeInformation>();

        public ConstructorInfo ResolveConstructor(Type type, bool nonPublicConstructors)
        {
            var preferredConstructor = ReflectionUtils.GetConstructor(type, typeof(PreferredConstructorAttribute), nonPublicConstructors);
            if (preferredConstructor != null)
            {
                return preferredConstructor;
            }

            var emptyConstructor = ReflectionUtils.GetEmptyConstructor(type, nonPublicConstructors);
            if (emptyConstructor != null)
                return emptyConstructor;

            var constructor = ReflectionUtils.GetDefaultConstructor(type, nonPublicConstructors);
            return constructor;
        }

        public TypeInformation GetTypeInformation(Type type, bool nonPublicConstructors)
        {
            if (typesCache.ContainsKey(type))
            {
                return typesCache[type];
            }
            else
            {
                var constructor = ResolveConstructor(type, nonPublicConstructors);
                if (constructor == null) { throw new ResolutionFailedException("No constructor found for \"" + type.Name + "\""); }

                var parameters = constructor.GetParameters();
                var typeInfo = new TypeInformation(constructor, parameters);
                typesCache[type] = typeInfo;
                return typeInfo;
            }
        }

        public bool ContainsKey(Type type)
        {
            return this.typesCache.ContainsKey(type);
        }

        public void Remove(Type type)
        {
            this.typesCache.Remove(type);
        }

        public void Clear()
        {
            this.typesCache.Clear();
        }
    }
}
