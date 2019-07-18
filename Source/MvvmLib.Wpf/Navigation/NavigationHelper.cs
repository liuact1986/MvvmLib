using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Helper for <see cref="NavigationSource"/> and <see cref="SharedSource{T}"/>.
    /// </summary>
    public class NavigationHelper
    {
        /// <summary>
        /// Checks if the type is <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if the type is the type is FrameworkElement</returns>
        public static bool IsFrameworkElementType(Type type)
        {
            var isFrameworkELementType = typeof(FrameworkElement).IsAssignableFrom(type);
            return isFrameworkELementType;
        }

        /// <summary>
        /// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="navigationContext">The navigation context</param>
        public static bool CanDeactivate(object source, NavigationContext navigationContext)
        {
            var view = source as FrameworkElement;
            if (view != null)
            {
                if (view is ICanDeactivate)
                {
                    if (!((ICanDeactivate)view).CanDeactivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }

                if (view.DataContext is ICanDeactivate)
                {
                    if (!((ICanDeactivate)view.DataContext).CanDeactivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }
            }
            else
            {
                if (source is ICanDeactivate)
                {
                    if (!((ICanDeactivate)source).CanDeactivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="navigationContext">The navigation context</param>
        public static bool CanActivate(object source, NavigationContext navigationContext)
        {
            var view = source as FrameworkElement;
            if (view != null)
            {
                if (view is ICanActivate)
                {
                    if (!((ICanActivate)view).CanActivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }

                if (view.DataContext is ICanActivate)
                {
                    if (!((ICanActivate)view.DataContext).CanActivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }
            }
            else
            {
                if (source is ICanActivate)
                {
                    if (!((ICanActivate)source).CanActivate(navigationContext).GetAwaiter().GetResult())
                        return false;
                }
            }

            return true;
        }


        ///// <summary>
        ///// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>.
        ///// </summary>
        ///// <param name="source">The source</param>
        ///// <param name="navigationContext">The navigation context</param>
        //public static async Task<bool> CanDeactivate(object source, NavigationContext navigationContext)
        //{
        //    var view = source as FrameworkElement;
        //    if (view != null)
        //    {
        //        if (view is ICanDeactivate)
        //        {
        //            if (!await ((ICanDeactivate)view).CanDeactivate(navigationContext))
        //                return false;
        //        }

        //        if (view.DataContext is ICanDeactivate)
        //        {
        //            if (!await ((ICanDeactivate)view.DataContext).CanDeactivate(navigationContext))
        //                return false;
        //        }
        //    }
        //    else
        //    {
        //        if (source is ICanDeactivate)
        //        {
        //            if (!await ((ICanDeactivate)source).CanDeactivate(navigationContext))
        //                return false;
        //        }
        //    }

        //    return true;
        //}

        ///// <summary>
        ///// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        ///// </summary>
        ///// <param name="source">The source</param>
        ///// <param name="navigationContext">The navigation context</param>
        //public static async Task<bool> CanActivate(object source, NavigationContext navigationContext)
        //{
        //    var view = source as FrameworkElement;
        //    if (view != null)
        //    {
        //        if (view is ICanActivate)
        //        {
        //            if (!await ((ICanActivate)view).CanActivate(navigationContext))
        //                return false;
        //        }

        //        if (view.DataContext is ICanActivate)
        //        {
        //            if (!await ((ICanActivate)view.DataContext).CanActivate(navigationContext))
        //                return false;
        //        }
        //    }
        //    else
        //    {
        //        if (source is ICanActivate)
        //        {
        //            if (!await ((ICanActivate)source).CanActivate(navigationContext))
        //                return false;
        //        }
        //    }

        //    return true;
        //}


        /// <summary>
        /// Tries to find existing source that implement <see cref="ISelectable"/> and that is target.
        /// </summary>
        /// <param name="sources">The sources</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The selectable found or null</returns>
        public static object FindSelectable(IEnumerable sources, Type sourceType, object parameter)
        {
            foreach (var source in sources)
            {
                if (source.GetType() == sourceType)
                {
                    if (source is FrameworkElement)
                    {
                        var view = source as FrameworkElement;
                        if (view.DataContext is ISelectable)
                        {
                            if (((ISelectable)view.DataContext).IsTarget(sourceType, parameter))
                                return source;
                        }
                    }
                    else
                    {
                        if (source is ISelectable)
                        {
                            if (((ISelectable)source).IsTarget(sourceType, parameter))
                                return source;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Creates an instance with the <see cref="SourceResolver"/>. 
        /// Allows to resolve dependencies if the factory is overridden with an IoC Container.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The instance created</returns>
        public static object CreateNew(Type sourceType)
        {
            var source = SourceResolver.CreateInstance(sourceType);
            return source;
        }


        /// <summary>
        /// Resolves the view model with <see cref="ViewModelLocator"/>.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <returns>The view model resolved or null</returns>
        public static object ResolveViewModelWithViewModelLocator(Type viewType)
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType); // singleton or new instance
            if (viewModelType != null)
            {
                var context = ViewModelLocationProvider.CreateViewModelInstance(viewModelType);
                return context;
            }
            return null;
        }

        /// <summary>
        /// Notifies ViewModels <see cref="OnNavigatingFrom(object, NavigationContext)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="navigationContext">The navigation context</param>
        public static void OnNavigatingFrom(object current, NavigationContext navigationContext)
        {
            if (current == null)
                return;

            if (current is FrameworkElement)
            {
                var view = current as FrameworkElement;
                if (view.DataContext is INavigationAware)
                    ((INavigationAware)view.DataContext).OnNavigatingFrom(navigationContext);
            }
            else if (current is INavigationAware)
                ((INavigationAware)current).OnNavigatingFrom(navigationContext);
        }

        /// <summary>
        /// Notifies ViewModels <see cref="OnNavigatingTo(object, NavigationContext)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="navigationContext">The navigation context</param>
        public static void OnNavigatingTo(object viewModel, NavigationContext navigationContext)
        {
            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).OnNavigatingTo(navigationContext);
        }

        /// <summary>
        /// Notifies ViewModels <see cref="OnNavigatedTo(object, NavigationContext)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="navigationContext">The navigation context</param>
        public static void OnNavigatedTo(object viewModel, NavigationContext navigationContext)
        {
            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).OnNavigatedTo(navigationContext);
        }

        /// <summary>
        /// Creates the source and sets the data context for view.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The source created</returns>
        public static object ResolveSource(Type sourceType)
        {
            var source = CreateNew(sourceType);
            var view = source as FrameworkElement;
            if (view != null && view.DataContext == null)
                view.DataContext = ResolveViewModelWithViewModelLocator(sourceType);
            return source;
        }

        /// <summary>
        /// Ensures creates a new instance for <see cref="FrameworkElement"/>. Avoid binding troubles.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>The new view or the original source</returns>
        public static object EnsureNewView(object source)
        {
            if (source is FrameworkElement)
            {
                var frameworkElement = source as FrameworkElement;
                // create view instance
                var view = CreateNew(source.GetType()) as FrameworkElement;
                // set data context
                view.DataContext = frameworkElement.DataContext;
                return view;
            }
            else
                return source;
        }
    }
}
