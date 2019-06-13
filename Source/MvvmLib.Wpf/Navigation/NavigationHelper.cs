using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Helper class for <see cref="NavigationSource"/> and <see cref="SharedSource{T}"/>.
    /// </summary>
    public class NavigationHelper
    {
        /// <summary>
        /// Checks if the type is <see cref="FrameworkElement"/> type.
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True or false</returns>
        public static bool IsFrameworkElementType(Type type)
        {
            var isFrameworkELementType = typeof(FrameworkElement).IsAssignableFrom(type);
            return isFrameworkELementType;
        }

        /// <summary>
        /// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="viewModel">The view model</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if can activate</returns>
        public static async Task<bool> CanActivateAsync(FrameworkElement view, object viewModel, object parameter)
        {
            if (view is ICanActivate)
                if (!await ((ICanActivate)view).CanActivateAsync(parameter))
                    return false;

            if (viewModel is ICanActivate)
                if (!await ((ICanActivate)viewModel).CanActivateAsync(parameter))
                    return false;

            return true;
        }

        /// <summary>
        /// Checks activation for view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if can activate</returns>
        public static async Task<bool> CanActivateAsync(object viewModel, object parameter)
        {
            if (viewModel is ICanActivate)
                if (!await ((ICanActivate)viewModel).CanActivateAsync(parameter))
                    return false;

            return true;
        }

        /// <summary>
        /// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>. The view is checked before the ViewModel.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <returns>True if can deactivate</returns>
        public static async Task<bool> CanDeactivateAsync(object current)
        {
            if (current == null)
                return true;
            // 1. View => 2. ViewModel
            // Source is View or ViewModel
            if (current is ICanDeactivate)
                if (!await ((ICanDeactivate)current).CanDeactivateAsync())
                    return false;

            // Check CanDeactivate for the viewModel of a View
            if (current is FrameworkElement)
            {
                var view = current as FrameworkElement;
                if (view.DataContext is ICanDeactivate)
                    if (!await ((ICanDeactivate)view.DataContext).CanDeactivateAsync())
                        return false;
            }

            return true;
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
        /// Notifies ViewModels <see cref="OnNavigatingFrom(object)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        public static void OnNavigatingFrom(object current)
        {
            if (current == null)
                return;

            if (current is FrameworkElement)
            {
                var view = current as FrameworkElement;
                if (view.DataContext is INavigationAware)
                    ((INavigationAware)view.DataContext).OnNavigatingFrom();
            }
            else if (current is INavigationAware)
                ((INavigationAware)current).OnNavigatingFrom();
        }

        /// <summary>
        /// Notifies ViewModels <see cref="OnNavigatingTo(object, object)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="parameter">The parameter</param>
        public static void OnNavigatingTo(object viewModel, object parameter)
        {
            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).OnNavigatingTo(parameter);
        }

        /// <summary>
        /// Notifies ViewModels <see cref="OnNavigatedTo(object, object)"/> that implement <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <param name="parameter">The parameter</param>
        public static void OnNavigatedTo(object viewModel, object parameter)
        {
            if (viewModel is INavigationAware)
                ((INavigationAware)viewModel).OnNavigatedTo(parameter);
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
                    if (source is ISelectable)
                    {
                        if (((ISelectable)source).IsTarget(sourceType, parameter))
                            return source;
                    }
                    if (source is FrameworkElement)
                    {
                        var frameworkElement = source as FrameworkElement;
                        if (frameworkElement.DataContext is ISelectable)
                        {
                            if (((ISelectable)frameworkElement.DataContext).IsTarget(sourceType, parameter))
                                return source;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sources">The sources</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <returns>True if success</returns>
        public static async Task<bool> NavigateAsync(object current, IEnumerable sources, Type sourceType, object parameter, Action<object> setCurrent)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (await CanDeactivateAsync(current))
            {
                var selectable = FindSelectable(sources, sourceType, parameter);
                if (selectable != null)
                    return await EndNavigateWithSelectableAsync(current, selectable, parameter, setCurrent);

                return await EndNavigateAsync(current, sourceType, parameter, setCurrent);
            }

            return false;
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/>. <see cref="ICanDeactivate"/> is not checked.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <returns>True if success</returns>
        public static async Task<bool> EndNavigateAsync(object current, Type sourceType, object parameter, Action<object> setCurrent)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (setCurrent == null)
                throw new ArgumentNullException(nameof(setCurrent));

            var source = CreateNew(sourceType);
            var view = source as FrameworkElement;
            if (view != null)
            {
                if (view.DataContext == null)
                    view.DataContext = ResolveViewModelWithViewModelLocator(view.GetType());

                object context = view.DataContext;

                if (await CanActivateAsync(view, context, parameter))
                {
                    OnNavigatingFrom(current);

                    OnNavigatingTo(context, parameter);

                    setCurrent(view);

                    OnNavigatedTo(context, parameter);

                    return true;
                }
            }
            else
            {
                if (await CanActivateAsync(source, parameter))
                {
                    OnNavigatingFrom(current);

                    OnNavigatingTo(source, parameter);

                    setCurrent(source);

                    OnNavigatedTo(source, parameter);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Processes navigation with selectable. <see cref="ICanDeactivate"/> is not checked.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="selectable">The selectable</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <returns>True if success</returns>
        public static async Task<bool> EndNavigateWithSelectableAsync(object current, object selectable, object parameter, Action<object> setCurrent)
        {
            if (selectable is FrameworkElement)
            {
                var frameworkElement = selectable as FrameworkElement;
                if (await CanActivateAsync(frameworkElement, frameworkElement.DataContext, parameter))
                {
                    OnNavigatingFrom(current);

                    setCurrent(selectable);

                    return true;
                }
            }
            else
            {
                if (await CanActivateAsync(selectable, parameter))
                {
                    OnNavigatingFrom(current);

                    setCurrent(selectable);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replaces the current source by the new source.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="source">The new source</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <returns>True if success</returns>
        public static async Task<bool> ReplaceAsync(object current, object source, object parameter, Action setCurrent)
        {
            if (await CanDeactivateAsync(current))
            {
                var view = source as FrameworkElement;
                if (view != null)
                {
                    object context = view.DataContext;

                    if (await CanActivateAsync(view, context, parameter))
                    {
                        OnNavigatingFrom(current);

                        OnNavigatingTo(context, parameter);

                        setCurrent();

                        OnNavigatedTo(context, parameter);

                        return true;
                    }
                }
                else
                {
                    if (await CanActivateAsync(source, parameter))
                    {
                        OnNavigatingFrom(current);

                        OnNavigatingTo(source, parameter);

                        setCurrent();

                        OnNavigatedTo(source, parameter);

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Ensures that source is new instance for <see cref="FrameworkElement"/>. Avoid binding problem for UI.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>The new source or the original source</returns>
        public static object EnsureNew(object source)
        {
            if (source is FrameworkElement)
            {
                // create view instance
                var frameworkElement = source as FrameworkElement;
                // create view instance
                var view = SourceResolver.CreateInstance(source.GetType()) as FrameworkElement;
                // set data context
                view.DataContext = frameworkElement.DataContext;
                return view;
            }
            else
                return source;
        }
    }
}
