using System.Collections.Generic;

namespace MvvmLib.IoC
{
    /// <summary>
    /// Injector resolution extension methods.
    /// </summary>
    public static class IInjectorResolverExtensions
    {
        /// <summary>
        /// Fills the properties of the instance.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        public static T BuildUp<T>(this IInjectorResolver container, T instance) where T : class
        {
            return (T)container.BuildUp(instance);
        }

        /// <summary>
        /// Fills the properties of the instance.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name :key</param>
        /// <param name="instance">The instance</param>
        /// <returns>The instance filled</returns>
        public static T BuildUp<T>(this IInjectorResolver container, string name, T instance) where T : class
        {
            return (T)container.BuildUp(typeof(T), name, instance);
        }

        /// <summary>
        /// Fills the properties of the instance.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The instance filled</returns>
        public static T BuildUp<T>(this IInjectorResolver container) where T : class
        {
            return (T)container.BuildUp(typeof(T));
        }

        /// <summary>
        /// Gets the instance for the type and name / key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        public static T GetInstance<T>(this IInjectorResolver container, string name)
        {
            return (T)container.GetInstance(typeof(T), name);
        }

        /// <summary>
        /// Gets the instance for the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The instance</returns>
        public static T GetInstance<T>(this IInjectorResolver container)
        {
            return (T)container.GetInstance(typeof(T));
        }

        /// <summary>
        /// Gets a new instance for the type and name / key.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <param name="name">The name : key</param>
        /// <returns>The instance</returns>
        public static T GetNewInstance<T>(this IInjectorResolver container, string name)
        {
            return (T)container.GetNewInstance(typeof(T), name);
        }

        /// <summary>
        /// Gets a new instance for the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The instance</returns>
        public static T GetNewInstance<T>(this IInjectorResolver container)
        {
            return (T)container.GetNewInstance(typeof(T));
        }

        /// <summary>
        /// Gets all instances of the type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="container">The injector</param>
        /// <returns>The list of instances</returns>
        public static List<object> GetAllInstances<T>(this IInjectorResolver container)
        {
            return container.GetAllInstances(typeof(T));
        }
    }
}
