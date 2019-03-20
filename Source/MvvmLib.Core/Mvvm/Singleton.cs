using System;
using System.Collections.Concurrent;

namespace MvvmLib.Mvvm
{
    public sealed class Singleton<T> where T : new()
    {
        private static ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        private Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                return _instances.GetOrAdd(typeof(T), (t) => new T());
            }
        }
    }
}
