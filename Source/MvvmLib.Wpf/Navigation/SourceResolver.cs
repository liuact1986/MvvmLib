using System;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Resolver for types that require always a new instance.
    /// </summary>
    public class SourceResolver
    {
        private static Dictionary<string, Type> typesForNavigation;
        /// <summary>
        /// Types for navigation.
        /// </summary>
        public static IReadOnlyDictionary<string, Type> TypesForNavigation
        {
            get { return typesForNavigation; }
        }

        private static Func<Type, object> factory;

        static SourceResolver()
        {
            typesForNavigation = new Dictionary<string, Type>();
            SetFactoryToDefault();
        }

        /// <summary>
        /// Allows to change the default factory (Activator CreateInstance). This factory have to create a new instance each time. Do not use singleton.
        /// </summary>
        /// <param name="factory">The new view factory</param>
        public static void SetFactory(Func<Type, object> factory)
        {
            SourceResolver.factory = factory;
        }

        /// <summary>
        /// Creates an instance with the <see cref="factory"/> provided.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The instance</returns>
        public static object CreateInstance(Type sourceType)
        {
            return factory(sourceType);
        }

        /// <summary>
        /// Resets the <see cref="factory"/> to the default factory.
        /// </summary>
        public static void SetFactoryToDefault()
        {
            factory = sourceType => Activator.CreateInstance(sourceType);
        }

        /// <summary>
        /// Registers the type (View or ViewModel Type) for Navigation. Required only for navigation by name with Modules.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="sourceName">The sourceName</param>
        public static void RegisterTypeForNavigation<T>(string sourceName = null)
        {
            if (sourceName == null)
                sourceName = typeof(T).Name;

            typesForNavigation[sourceName] = typeof(T);
        }

        /// <summary>
        /// Clears the <see cref="TypesForNavigation"/>.
        /// </summary>
        public static void ClearTypesForNavigation()
        {
            typesForNavigation.Clear();
        }
    }
}