using System;
namespace MvvmLib.Navigation
{
    /// <summary>
    /// Resolver for types that require always a new instance.
    /// </summary>
    public class SourceResolver
    {
        private static Func<Type, object> factory;

        static SourceResolver()
        {
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
    }
}