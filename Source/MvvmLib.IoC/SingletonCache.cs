using MvvmLib.IoC.Registrations;
using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Cache for instances.
    /// </summary>
    public class SingletonCache
    {

        private readonly Dictionary<Type, Dictionary<string, object>> cache;
        /// <summary>
        /// The cache for instances.
        /// </summary>
        public Dictionary<Type, Dictionary<string, object>> Cache
        {
            get { return cache; }
        }

        /// <summary>
        /// Creates the sngleton cache class.
        /// </summary>
        public SingletonCache()
        {
            cache = new Dictionary<Type, Dictionary<string, object>>();
        }

        /// <summary>
        /// Adds a registration as singleton to the cache.
        /// </summary>
        /// <param name="registration">The registration</param>
        /// <param name="instance">The instance</param>
        /// <returns></returns>
        public bool TryAddToCache(TypeRegistration registration, object instance)
        {
            if (registration.IsSingleton)
            {
                if (!this.cache.ContainsKey(registration.TypeTo))
                    this.cache[registration.TypeTo] = new Dictionary<string, object>();

                this.cache[registration.TypeTo][registration.Name] = instance;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Checks if the cache contains the type with the name / key.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        /// <returns>True if cached</returns>
        public bool IsCached(Type type, string name)
        {
            return this.cache.ContainsKey(type) && this.cache[type].ContainsKey(name);
        }

        /// <summary>
        /// removes the instance with the key from the cache.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="name">The name / key</param>
        public void Remove(Type type, string name)
        {
            if (IsCached(type, name))
            {
                this.cache[type].Remove(name);
                if (this.cache[type].Count == 0)
                    this.cache.Remove(type);
            }
        }
    }
}
