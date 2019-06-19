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
        private static void CanActivateStep2(object item, object parameter, Action<bool> continuationCallback)
        {
            if (item is ICanActivate)
                ((ICanActivate)item).CanActivate(parameter, continuationCallback);
            else
                continuationCallback(true);
        }

        /// <summary>
        /// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanActivate(FrameworkElement view, object parameter, Action<bool> continuationCallback)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (continuationCallback == null)
                throw new ArgumentNullException(nameof(continuationCallback));

            if (view is ICanActivate)
                ((ICanActivate)view).CanActivate(parameter, canActivate =>
                {
                    if (canActivate)
                        CanActivateStep2(view.DataContext, parameter, continuationCallback);
                    else
                        continuationCallback(false);
                });
            else
                CanActivateStep2(view.DataContext, parameter, continuationCallback);
        }

        /// <summary>
        /// Checks activation for views and view models that implement <see cref="ICanActivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanActivate(object source, object parameter, Action<bool> continuationCallback)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (continuationCallback == null)
                throw new ArgumentNullException(nameof(continuationCallback));

            var view = source as FrameworkElement;
            if (view != null)
                CanActivate(view, parameter, continuationCallback);
            else
                CanActivateStep2(source, parameter, continuationCallback);
        }


        private static void CanDeactivateStep2(object item, Action<bool> continuationCallback)
        {
            if (item is ICanDeactivate)
                ((ICanDeactivate)item).CanDeactivate(continuationCallback);
            else
                continuationCallback(true);
        }

        /// <summary>
        /// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanDeactivate(object source, Action<bool> continuationCallback)
        {
            if (continuationCallback == null)
                throw new ArgumentNullException(nameof(continuationCallback));

            var view = source as FrameworkElement;
            if (view != null)
                CanDeactivate(view, continuationCallback);
            else
                CanDeactivateStep2(source, continuationCallback);
        }

        /// <summary>
        /// Checks deactivation for views and view models that implement <see cref="ICanDeactivate"/>.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CanDeactivate(FrameworkElement view, Action<bool> continuationCallback)
        {
            if (view == null)
                throw new ArgumentNullException(nameof(view));
            if (continuationCallback == null)
                throw new ArgumentNullException(nameof(continuationCallback));

            if (view is ICanDeactivate)
                ((ICanDeactivate)view).CanDeactivate(canDeactivate =>
                {
                    if (canDeactivate)
                        CanDeactivateStep2(view.DataContext, continuationCallback);
                    else
                        continuationCallback(false);
                });
            else
                CanDeactivateStep2(view.DataContext, continuationCallback);
        }

        #region INavigationAware

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

        #endregion // INavigationAware

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
                        var frameworkElement = source as FrameworkElement;
                        if (frameworkElement.DataContext is ISelectable)
                        {
                            if (((ISelectable)frameworkElement.DataContext).IsTarget(sourceType, parameter))
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
        /// Processes navigation for <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sources">The sources</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CheckGuardsAndNavigate(object current, IEnumerable sources, Type sourceType, object parameter, Action<object> setCurrent, Action<bool> continuationCallback)
        {
            if (sources == null)
                throw new ArgumentNullException(nameof(sources));
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            CanDeactivate(current, canDeactivate =>
            {
                if (canDeactivate)
                {
                    var selectable = FindSelectable(sources, sourceType, parameter);
                    if (selectable != null)
                        CheckCanActivateAndNavigateWithSelectable(current, selectable, parameter, setCurrent, continuationCallback);
                    else
                        CheckCanActivateAndNavigate(current, sourceType, parameter, setCurrent, continuationCallback);
                }
                else
                    continuationCallback(false);
            });
        }


        /// <summary>
        /// Processes navigation with selectable. <see cref="ICanDeactivate"/> is not checked.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="selectable">The selectable</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CheckCanActivateAndNavigateWithSelectable(object current, object selectable, object parameter, Action<object> setCurrent, Action<bool> continuationCallback)
        {
            CanActivate(selectable, parameter, canActivate =>
            {
                if (canActivate)
                    EndNavigate(current, selectable, setCurrent);

                continuationCallback(canActivate);
            });
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/>. <see cref="ICanDeactivate"/> is not checked.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void CheckCanActivateAndNavigate(object current, Type sourceType, object parameter, Action<object> setCurrent, Action<bool> continuationCallback)
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

                var context = view.DataContext;

                CanActivate(view, parameter, canActivate =>
                {
                    if (canActivate)
                        EndNavigate(current, source, context, parameter, setCurrent);

                    continuationCallback(canActivate);
                });
            }
            else
            {
                CanActivateStep2(source, parameter, canActivate =>
                {
                    if (canActivate)
                        EndNavigate(current, source, source, parameter, setCurrent);

                    continuationCallback(canActivate);
                });
            }
        }

        private static void EndNavigate(object current, object selectable, Action<object> setCurrent)
        {
            OnNavigatingFrom(current);
            setCurrent(selectable);
        }

        private static void EndNavigate(object current, object source, object context, object parameter, Action<object> setCurrent)
        {
            OnNavigatingFrom(current);

            OnNavigatingTo(context, parameter);

            setCurrent(source);

            OnNavigatedTo(context, parameter);
        }

        private static void EndNavigate(object current, object context, object parameter, Action setCurrent)
        {
            OnNavigatingFrom(current);

            OnNavigatingTo(context, parameter);

            setCurrent();

            OnNavigatedTo(context, parameter);
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="sources">The sources</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        public static void EndNavigate(object current, IEnumerable sources, Type sourceType, object parameter, Action<object> setCurrent)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var selectable = FindSelectable(sources, sourceType, parameter);
            if (selectable != null)
                EndNavigate(current, selectable, setCurrent);
            else
            {
                var source = CreateNew(sourceType);
                var view = source as FrameworkElement;
                if (view != null)
                {
                    if (view.DataContext == null)
                        view.DataContext = ResolveViewModelWithViewModelLocator(view.GetType());

                    var context = view.DataContext;
                    EndNavigate(current, source, context, parameter, setCurrent);
                }
                else
                    EndNavigate(current, source, source, parameter, setCurrent);
            }
        }

        /// <summary>
        /// Replaces the current source by the new source.
        /// </summary>
        /// <param name="current">The current source</param>
        /// <param name="source">The new source</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="setCurrent">The method invoked to change current source</param>
        /// <param name="continuationCallback">The continuation callback</param>
        public static void Replace(object current, object source, object parameter, Action setCurrent, Action<bool> continuationCallback)
        {
            CanDeactivate(current, canDeactivate =>
            {
                if (canDeactivate)
                {
                    var view = source as FrameworkElement;
                    if (view != null)
                    {
                        CanActivate(view, parameter, canActivate =>
                        {
                            if (canActivate)
                                EndNavigate(current, view.DataContext, parameter, setCurrent);

                            continuationCallback(canActivate);
                        });
                    }
                    else
                    {
                        CanActivate(source, parameter, canActivate =>
                        {
                            if (canActivate)
                                EndNavigate(current, source, parameter, setCurrent);

                            continuationCallback(canActivate);
                        });
                    }
                }
                else
                    continuationCallback(false);
            });
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
