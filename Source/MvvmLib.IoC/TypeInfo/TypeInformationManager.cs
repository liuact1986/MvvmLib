using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allwos to manage <see cref="TypeInformation"/> and <see cref="PropertyWithDependencyAttribute"/>.
    /// </summary>
    public sealed class TypeInformationManager
    {
        private readonly Dictionary<Type, TypeInformation> typeCache;
        /// <summary>
        /// The type cache.
        /// </summary>
        public Dictionary<Type, TypeInformation> TypeCache
        {
            get { return typeCache; }
        }

        private readonly Dictionary<Type, List<PropertyWithDependencyAttribute>> propertiesWithDependencyAttributeCache;
        /// <summary>
        /// the properties cache.
        /// </summary>
        public Dictionary<Type, List<PropertyWithDependencyAttribute>> PropertiesWithDependencyAttributeCache
        {
            get { return propertiesWithDependencyAttributeCache; }
        }

        /// <summary>
        /// Creates the type information manager class.
        /// </summary>
        public TypeInformationManager()
        {
            this.typeCache = new Dictionary<Type, TypeInformation>();
            this.propertiesWithDependencyAttributeCache = new Dictionary<Type, List<PropertyWithDependencyAttribute>>();
        }

        /// <summary>
        /// Gets the constructor (with <see cref="PreferredConstructorAttribute"/> or empty or first constructor) for the type. 
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublicConstructors">Allow to find non public constructors</param>
        /// <returns>The constructor info</returns>
        public ConstructorInfo GetConstructor(Type type, bool nonPublicConstructors)
        {
            var constructors = ReflectionUtils.GetConstructors(type, nonPublicConstructors);

            // preferred ctor
            var preferredConstructor = ReflectionUtils.GetConstructorWithAttribute(constructors, typeof(PreferredConstructorAttribute));
            if (preferredConstructor != null)
                return preferredConstructor;

            // empty ctor 
            var emptyConstructor = ReflectionUtils.GetEmptyConstructor(type, nonPublicConstructors);
            if (emptyConstructor != null)
                return emptyConstructor;

            // first ctor
            if (constructors.Length > 0)
                return constructors[0];
            else
                return null;
        }

        /// <summary>
        /// Gets the type information.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublicConstructors">Allow to find non public constructors</param>
        /// <returns>The type information</returns>
        public TypeInformation GetTypeInformation(Type type, bool nonPublicConstructors)
        {
            if (this.typeCache.TryGetValue(type, out TypeInformation typeInformation))
                return typeInformation;
            else
            {
                var constructor = GetConstructor(type, nonPublicConstructors);
                if (constructor == null)
                    throw new ResolutionFailedException($"Unable to resolve a constructor for type \"{type.Name}\" with non public \"{nonPublicConstructors}\"");

                var parameters = constructor.GetParameters();

                typeInformation = new TypeInformation(constructor, parameters);
                typeCache[type] = typeInformation;
                return typeInformation;
            }
        }

        /// <summary>
        /// Get the properties with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="nonPublicProperties">Allows to get non public properties</param>
        /// <returns>The list of properties</returns>
        public List<PropertyWithDependencyAttribute> GetPropertiesWithDependencyAttribute(Type type, bool nonPublicProperties)
        {
            if (this.propertiesWithDependencyAttributeCache.TryGetValue(type, out List<PropertyWithDependencyAttribute> propertiesWithDependencyAttribute))
                return propertiesWithDependencyAttribute;
            else
            {
                propertiesWithDependencyAttribute = new List<PropertyWithDependencyAttribute>();

                var properties = ReflectionUtils.GetProperties(type, nonPublicProperties);
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        var attribute = property.GetCustomAttribute(typeof(DependencyAttribute)) as DependencyAttribute;
                        if (attribute != null)
                        {
                            // var name = attribute.Name != null ? attribute.Name : property.Name;
                            var propertyWithDependencyAttribute = new PropertyWithDependencyAttribute(property, attribute.Name);
                            propertiesWithDependencyAttribute.Add(propertyWithDependencyAttribute);
                        }
                    }
                }

                this.propertiesWithDependencyAttributeCache[type] = propertiesWithDependencyAttribute;
                return propertiesWithDependencyAttribute;
            }
        }
    }
}
