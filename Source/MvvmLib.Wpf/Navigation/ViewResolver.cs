using System;
namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to create instances of views.
    /// </summary>
    public class ViewResolver
    {
        private static Func<Type, object> viewFactory = (sourceType) => Activator.CreateInstance(sourceType);

        /// <summary>
        /// Allows to change the default view factory (Activator CreateInstance).
        /// </summary>
        /// <param name="viewFactory">The new view factory</param>
        public static void SetViewFactory(Func<Type, object> viewFactory)
        {
            ViewResolver.viewFactory = viewFactory;
        }

        internal static object CreateInstance(Type sourceType)
        {
            return viewFactory(sourceType);
        }
    }
}