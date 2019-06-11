using System;
namespace MvvmLib.Navigation
{
    /// <summary>
    /// Resolver for types that require always a new instance.
    /// </summary>
    public class SourceResolver
    {
        private static Func<Type, object> factory = (sourceType) => Activator.CreateInstance(sourceType);

        /// <summary>
        /// Allows to change the default factory (Activator CreateInstance). This factory have to create a new instance each time. Do not use singleton.
        /// </summary>
        /// <param name="factory">The new view factory</param>
        public static void SetFactory(Func<Type, object> factory)
        {
            SourceResolver.factory = factory;
        }

        internal static object CreateInstance(Type sourceType)
        {
            return factory(sourceType);
        }
    }
}