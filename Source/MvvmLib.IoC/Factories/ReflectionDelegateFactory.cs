using System;
using System.Reflection;

namespace MvvmLib.IoC.Factories
{
    /// <summary>
    /// Allows to gets instance factories with <see cref="System.Reflection"/>.
    /// </summary>
    public sealed class ReflectionDelegateFactory : IDelegateFactory
    {
        /// <summary>
        /// Creates a factory for an empty constructor.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        public Func<T> CreateConstructor<T>(Type type, ConstructorInfo constructor)
        {
            return () => (T)constructor.Invoke(null);
        }

        /// <summary>
        /// Creates a factory for a constructor with parameters.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        public Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo constructor)
        {
            return (p) => (T)constructor.Invoke(p);
        }
    }
}
