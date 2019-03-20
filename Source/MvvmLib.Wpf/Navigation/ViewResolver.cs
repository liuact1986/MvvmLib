using System;


namespace MvvmLib.Navigation
{
    /// <summary>
    /// Creates an instance of a view. 
    /// </summary>
    public class ViewResolver
    {
        static Func<Type, object> viewFactory = (viewType) => Activator.CreateInstance(viewType);

        /// <summary>
        /// The view factory receives a view type and have to get always a new instance of a view.
        /// </summary>
        /// <param name="viewFactory">The view factory</param>
        public static void SetViewFactory(Func<Type, object> viewFactory)
        {
            ViewResolver.viewFactory = viewFactory;
        }

        public static object Resolve(Type viewType)
        {
            return viewFactory(viewType);
        }
    }
}