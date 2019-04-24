using System;
using System.Reflection;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Allows to gets instance factories.
    /// </summary>
    public interface IDelegateFactory
    {
        /// <summary>
        /// Creates a factory for an empty constructor.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        Func<T> CreateConstructor<T>(Type type, ConstructorInfo constructor);

        /// <summary>
        /// Creates a factory for a constructor with parameters.
        /// </summary>
        /// <typeparam name="T">The type of instance</typeparam>
        /// <param name="type">The type of instance</param>
        /// <param name="constructor">The constructor info</param>
        /// <returns>The factory</returns>
        Func<object[], T> CreateParameterizedConstructor<T>(Type type, ConstructorInfo constructor);
    }
}