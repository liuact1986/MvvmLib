using System;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    public class InstancesCache
    {
        protected Dictionary<Type, Dictionary<string, object>> instancesCache
            = new Dictionary<Type, Dictionary<string, object>>();

        public bool TryAddToCache(TypeRegistration registration, object instance)
        {
            if (registration.IsSingleton)
            {
                if (!this.instancesCache.ContainsKey(registration.TypeFrom))
                {
                    this.instancesCache[registration.TypeFrom] = new Dictionary<string, object>();
                }
                this.instancesCache[registration.TypeFrom][registration.Name] = instance;
                return true;
            }
            return false;
        }

        public bool IsTypeCached(Type type)
        {
            return this.instancesCache.ContainsKey(type);
        }

        public bool IsCached(Type type, string name)
        {
            return this.instancesCache.ContainsKey(type)
               && this.instancesCache[type].ContainsKey(name);
        }

        public object GetFromCache(Type type, string name)
        {
            return instancesCache[type][name];
        }

        public void Remove(Type type)
        {
            this.instancesCache.Remove(type);
        }

        public void Remove(Type type, string name)
        {
            this.instancesCache[type].Remove(name);
        }

        public void Clear()
        {
            this.instancesCache.Clear();
        }
    }

}