using System;
using System.Collections;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Helper class for <see cref="NavigationSource"/> and <see cref="SharedSource{T}"/>.
    /// </summary>
    public class NavigationHelper
    {
        private static void CanDeactivateStep2(object source, NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (source is ICanDeactivate)
                ((ICanDeactivate)source).CanDeactivate(navigationContext, continuationCallback);
            else
                continuationCallback(true);
        }

        /// <summary>
        /// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanDeactivate(object source, NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var view = source as FrameworkElement;
            if (view != null)
                if (view is ICanDeactivate)
                {
                    ((ICanDeactivate)view).CanDeactivate(navigationContext, canDeactivate =>
                    {
                        if (canDeactivate)
                            CanDeactivateStep2(view.DataContext, navigationContext, continuationCallback);
                        else
                            continuationCallback(false);
                    });
                }
                else
                    CanDeactivateStep2(view.DataContext, navigationContext, continuationCallback);
            else
                CanDeactivateStep2(source, navigationContext, continuationCallback);
        }

        private static void CanActivateStep2(object source, NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (source is ICanActivate)
                ((ICanActivate)source).CanActivate(navigationContext, continuationCallback);
            else
                continuationCallback(true);
        }

        /// <summary>
        /// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanActivate(object source, NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var view = source as FrameworkElement;
            if (view != null)
                if (view is ICanActivate)
                {
                    ((ICanActivate)view).CanActivate(navigationContext, canActivate =>
                    {
                        if (canActivate)
                            CanActivateStep2(view.DataContext, navigationContext, continuationCallback);
                        else
                            continuationCallback(false);
                    });
                }
                else
                    CanActivateStep2(view.DataContext, navigationContext, continuationCallback);
            else
                CanActivateStep2(source, navigationContext, continuationCallback);
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
        /// Processes navigation for <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sources">The sources</param>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void Navigate(object current, IEnumerable sources, NavigationContext navigationContext, Action<object, bool> setCurrent, Action<bool> continuationCallback)
        {
            var sourceType = navigationContext.SourceType;
            var parameter = navigationContext.Parameter;
            CanDeactivate(current, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                {
                    var selectable = FindSelectable(sources, sourceType, parameter);
                    if (selectable != null)
                    {
                        CanActivate(selectable, navigationContext, canActivate =>
                        {
                            if (canActivate)
                            {
                                OnNavigatingFrom(current, navigationContext);
                                setCurrent(selectable, true);
                            }
                            continuationCallback(canActivate);
                        });
                    }
                    else
                    {
                        var source = CreateNew(sourceType);
                        var view = source as FrameworkElement;
                        if (view != null && view.DataContext == null)
                            view.DataContext = ResolveViewModelWithViewModelLocator(sourceType);

                        CanActivate(source, navigationContext, canActivate =>
                        {
                            if (canActivate)
                            {
                                if (view != null)
                                    ExecuteNavigation(current, source, view.DataContext, navigationContext, setCurrent);
                                else
                                    ExecuteNavigation(current, source, navigationContext, setCurrent);
                            }
                            continuationCallback(canActivate);
                        });
                    }
                }
                else
                    continuationCallback(false);
            });
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sources">The sources</param>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        public static void NavigateFast(object current, IEnumerable sources, NavigationContext navigationContext, Action<object, bool> setCurrent)
        {
            var sourceType = navigationContext.SourceType;
            var parameter = navigationContext.Parameter;

            var selectable = FindSelectable(sources, sourceType, parameter);
            if (selectable != null)
            {
                OnNavigatingFrom(current, navigationContext);
                setCurrent(selectable, true);
            }
            else
            {
                var source = CreateNew(sourceType);
                var view = source as FrameworkElement;
                if (view != null && view.DataContext == null)
                    view.DataContext = ResolveViewModelWithViewModelLocator(sourceType);

                if (view != null)
                    ExecuteNavigation(current, source, view.DataContext, navigationContext, setCurrent);
                else
                    ExecuteNavigation(current, source, navigationContext, setCurrent);
            }
        }

        /// <summary>
        /// Replaces the current source by the new source.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="source">The new source</param>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void Replace(object current, object source, NavigationContext navigationContext, Action setCurrent, Action<bool> continuationCallback)
        {
            CanDeactivate(current, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                {
                    CanActivate(source, navigationContext, canActivate =>
                    {
                        if (canActivate)
                        {
                            var view = source as FrameworkElement;
                            if (view != null)
                                ExecuteNavigation(current, source, view.DataContext, navigationContext, (s, t) => setCurrent());
                            else
                                ExecuteNavigation(current, source, navigationContext, (s, t) => setCurrent());
                        }
                        continuationCallback(canActivate);
                    });
                }
                else
                    continuationCallback(false);
            });
        }

        private static void ExecuteNavigation(object current, object source, object context, NavigationContext navigationContext, Action<object, bool> setCurrent)
        {
            OnNavigatingFrom(current, navigationContext);

            OnNavigatingTo(context, navigationContext);

            setCurrent(source, false);

            OnNavigatedTo(context, navigationContext);
        }

        private static void ExecuteNavigation(object current, object source, NavigationContext navigationContext, Action<object, bool> setCurrent)
        {
            ExecuteNavigation(current, source, source, navigationContext, setCurrent);
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
