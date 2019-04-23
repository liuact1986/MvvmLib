using System;
using System.Collections.Concurrent;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to create quickly a singleton with <see cref="ConcurrentDictionary{TKey, TValue}"/>
    /// </summary>
    /// <typeparam name="T">The type of class</typeparam>
    public sealed class Singleton<T> where T : new()
    {
        private static ConcurrentDictionary<Type, T> _instances = new ConcurrentDictionary<Type, T>();

        private Singleton()
        {
        }

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                return _instances.GetOrAdd(typeof(T), (t) => new T());
            }
        }
    }
}
