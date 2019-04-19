using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Reghion base class.
    /// </summary>
    public abstract class RegionBase : IRegion
    {
        private readonly ILogger DefaultLogger = new DebugLogger();

        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName { get; protected set; }

        /// <summary>
        /// The control for this region.
        /// </summary>
        public object Control { get; }

        /// <summary>
        /// The name of the control. Can be used to get a region by region name and control name.
        /// </summary>
        public string ControlName => ((FrameworkElement)Control).Name;

        /// <summary>
        /// Checks if region is loaded.
        /// </summary>
        protected internal bool isLoaded;

        /// <summary>
        /// Gets the current entry of history.
        /// </summary>
        public abstract NavigationEntry CurrentEntry { get; }


        /// <summary>
        /// The region logger.
        /// </summary>
        protected ILogger logger;

        /// <summary>
        /// The logger used by the Region
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        /// <summary>
        /// Navigating event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigating = new List<EventHandler<RegionNavigationEventArgs>>();
        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigating
        {
            add { if (!navigating.Contains(value)) navigating.Add(value); }
            remove { if (navigating.Contains(value)) navigating.Remove(value); }
        }

        /// <summary>
        /// Navigated event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigated = new List<EventHandler<RegionNavigationEventArgs>>();
        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        public event EventHandler<RegionNavigationEventArgs> Navigated
        {
            add { if (!navigated.Contains(value)) navigated.Add(value); }
            remove { if (navigated.Contains(value)) navigated.Remove(value); }
        }

        /// <summary>
        /// NavigatiionFailed event handlers list.
        /// </summary>
        protected readonly List<EventHandler<RegionNavigationFailedEventArgs>> navigationFailed = new List<EventHandler<RegionNavigationFailedEventArgs>>();
        /// <summary>
        /// Invoked on navigation cancelled or on exception.
        /// </summary>
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed
        {
            add { if (!navigationFailed.Contains(value)) navigationFailed.Add(value); }
            remove { if (navigationFailed.Contains(value)) navigationFailed.Remove(value); }
        }

        /// <summary>
        /// Creates the region.
        /// </summary>
        /// <param name="regionName"></param>
        /// <param name="control"></param>
        public RegionBase(string regionName, object control)
        {
            this.RegionName = regionName;
            this.Control = control;
        }

        /// <summary>
        /// Creates a new instance with the view resolver.
        /// </summary>
        /// <param name="viewOrObjectType">The type of the instance</param>
        /// <returns>The instance</returns>
        protected object CreateViewOrObjectInstance(Type viewOrObjectType)
        {
            var viewOrObject = ViewResolver.Resolve(viewOrObjectType);
            return viewOrObject;
        }

        #region Deactivatable management


        /// <summary>
        /// Check Can Deactivate for child regions.
        /// </summary>
        /// <param name="childRegions">The child regions</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateChildRegionsAsync(IList<RegionBase> childRegions)
        {
            // sub child => child => parent
            foreach (var childRegion in childRegions)
            {
                var currentEntry = childRegion.CurrentEntry;
                if (currentEntry != null)
                {
                    // child 
                    if (currentEntry.ChildRegions.Count > 0)
                        await childRegion.CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions);

                    // current
                    await childRegion.CheckCanDeactivateAsync(currentEntry);
                }
            }
        }

        /// <summary>
        /// Check Can Deactivate for child regions and current entry.
        /// </summary>
        /// <param name="currentEntry">The current navigation entry.</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateAsync(NavigationEntry currentEntry)
        {
            // sub child => child => parent
            if (currentEntry.ChildRegions.Count > 0)
                await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions);

            var viewOrObject = currentEntry.ViewOrObject;
            if (viewOrObject != null && viewOrObject is IDeactivatable viewOrObjectAsIDeactivatable)
                if (!await viewOrObjectAsIDeactivatable.CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);


            var context = currentEntry.Context;
            if (context != null && context is IDeactivatable contextAsIDeactivatable)
                if (!await contextAsIDeactivatable.CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }


        #endregion // Deactivatable management

        #region IActivatable management

        protected async Task CheckCanActivateAsync(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is IActivatable viewOrObjectAsIActivatable)
                if (!await viewOrObjectAsIActivatable.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);

            if (context != null && context is IActivatable contextAsIActivatable)
                if (!await contextAsIActivatable.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }

        protected async Task CheckCanActivateAsync(NavigationEntry entry)
        {
            // parent => child => sub child
            var viewOrObject = entry.ViewOrObject;
            var parameter = entry.Parameter;
            if (viewOrObject != null && viewOrObject is IActivatable viewOrObjectAsIActivatable)
                if (!await viewOrObjectAsIActivatable.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.ViewOrObject, viewOrObject, this);

            var context = entry.Context;
            if (context != null && context is IActivatable contextAsIActivatable)
                if (!await contextAsIActivatable.CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);

            if (entry.ChildRegions.Count > 0)
                await CheckCanActivateChildRegionsAsync(entry.ChildRegions);
        }


        protected async Task CheckCanActivateChildRegionsAsync(IList<RegionBase> childRegions)
        {
            // parent => child => sub child
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    // current child
                    var viewOrObject = entry.ViewOrObject;
                    await childRegion.CheckCanActivateAsync(viewOrObject, entry.Context, entry.Parameter);


                    // sub child regions
                    if (entry.ChildRegions.Count > 0)
                        await childRegion.CheckCanActivateChildRegionsAsync(entry.ChildRegions);
                }
            }
        }

        #endregion // IActivatable management

        #region INavigatable management

        protected void OnNavigatingFrom(NavigationEntry entry)
        {
            // subs child => child => parent
            if (entry.ChildRegions.Count > 0)
                OnNavigatingFromChildRegions(entry.ChildRegions);

            var viewOrObject = entry.ViewOrObject;
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingFrom();

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingFrom();
        }

        protected void OnNavigatingFromChildRegions(IList<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    // sub child regions
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatingFromChildRegions(entry.ChildRegions);

                    // current child region
                    childRegion.OnNavigatingFrom(entry);
                }
            }
        }

        protected void OnNavigatingTo(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingTo(parameter);

            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingTo(parameter);
        }

        protected void OnNavigatingTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var viewOrObject = entry.ViewOrObject;
            var parameter = entry.Parameter;

            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatingTo(parameter);

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatingTo(parameter);

            if (entry.ChildRegions.Count > 0)
                OnNavigatingToChildRegions(entry.ChildRegions);
        }

        protected void OnNavigatingToChildRegions(IList<RegionBase> childRegions)
        {
            //  parent => child => sub child
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    childRegion.OnNavigatingTo(entry);

                    // sub child
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatingToChildRegions(entry.ChildRegions);
                }
            }
        }

        protected void OnNavigatedTo(object viewOrObject, object context, object parameter)
        {
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatedTo(parameter);

            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatedTo(parameter);
        }

        protected void OnNavigatedTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var viewOrObject = entry.ViewOrObject;
            var parameter = entry.Parameter;
            if (viewOrObject != null && viewOrObject is INavigatable viewOrObjectAsINavigatable)
                viewOrObjectAsINavigatable.OnNavigatedTo(parameter);

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                contextAsINavigatable.OnNavigatedTo(parameter);

            if (entry.ChildRegions.Count > 0)
                OnNavigatedToChildRegions(entry.ChildRegions);
        }

        protected void OnNavigatedToChildRegions(List<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var entry = childRegion.CurrentEntry;
                if (entry != null)
                {
                    childRegion.OnNavigatedTo(entry);

                    // sub child
                    if (entry.ChildRegions.Count > 0)
                        childRegion.OnNavigatedToChildRegions(entry.ChildRegions);
                }
            }
        }

        #endregion INavigatable management

        protected object ResolveContextWithViewModelLocator(Type viewType)
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType); // singleton or new instance
            if (viewModelType != null)
            {
                var context = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                return context;
            }
            return null;
        }

        protected List<RegionBase> FindChildRegions(DependencyObject parent)
        {
            var childRegions = new List<RegionBase>();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child.GetValue(RegionManager.ContentRegionNameProperty) != null)
                {
                    // find region
                    var regionName = child.GetValue(RegionManager.ContentRegionNameProperty) as string;
                    var region = RegionManager.FindContentRegion(regionName, child);
                    if (region != null)
                    {
                        childRegions.Add(region);
                    }
                }
                else if (child.GetValue(RegionManager.ItemsRegionNameProperty) != null)
                {
                    var regionName = child.GetValue(RegionManager.ItemsRegionNameProperty) as string;
                    var region = RegionManager.FindItemsRegion(regionName, child);
                    if (region != null)
                    {
                        childRegions.Add(region);
                    }
                }
                childRegions.AddRange(FindChildRegions(child));
            }
            return childRegions;

        }

        protected void ClearChildRegions(NavigationEntry entry)
        {
            // sub child => child => parent
            foreach (var child in entry.ChildRegions)
            {
                // sub child
                if (child.CurrentEntry != null && child.CurrentEntry.ChildRegions.Count > 0)
                    child.ClearChildRegions(child.CurrentEntry);

                if (child is ContentRegion)
                {
                    var contentRegion = child as ContentRegion;
                    contentRegion.ClearContent();
                    contentRegion.History.Clear();

                    RegionManager.UnregisterContentRegion(contentRegion);
                }
                else
                {
                    var itemsRegion = child as ItemsRegion;
                    itemsRegion.ClearItems();
                    itemsRegion.History.Clear();

                    RegionManager.UnregisterItemsRegion(itemsRegion);
                }
            }
        }


        protected void HandleLoaded(FrameworkElement view, Action<object, RoutedEventArgs> onLoaded)
        {
            // Called only after "control.Content = view"
            var listener = new FrameworkElementLoaderListener(view);
            listener.Subscribe((s, e) =>
            {
                listener.Unsubscribe();
                listener = null;

                onLoaded(s, e);
            });
        }

        protected void NotifyLoadedListeners(FrameworkElement view, object context, object parameter)
        {
            if (context != null && context is ILoadedEventListener loadedEventListener)
                loadedEventListener.OnLoaded(view, parameter);
        }

        protected Type GetGenericRegionKnowledge(Type contextType)
        {
            var interfaces = contextType.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IRegionKnowledge<>))
                {
                    var regionType = @interface.GetGenericArguments()[0];
                    return regionType;
                }
            }

            return null;
        }

        protected void NotifyRegionKnowledge(IRegion region, object context)
        {
            // IRegionKnowledge
            if (context is IRegionKnowledge regionKnowledge)
                regionKnowledge.GetRegion(region);

            // IRegionKnowledge<T>
            Type contextType = context.GetType();
            var regionType = GetGenericRegionKnowledge(contextType);
            if (regionType != null)
            {
                var method = contextType.GetMethod("GetRegion", new Type[] { regionType });
                if (method != null)
                {
                    if (region.GetType() != regionType)
                        throw new InvalidOperationException($"Invalid region type. Expected \"{regionType.Name}\", Current \"{region.GetType().Name}\"");

                    method.Invoke(context, new object[] { region });
                }
            }
        }

        protected void RaiseNavigating(Type viewType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(viewType, parameter, regionNavigationType);
            foreach (var handler in this.navigating)
                handler(this, context);
        }

        protected void RaiseNavigated(Type viewType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(viewType, parameter, regionNavigationType);
            foreach (var handler in this.navigated)
                handler(this, context);
        }

        protected void RaiseNavigationNavigationFailed(NavigationFailedException exception)
        {
            var context = new RegionNavigationFailedEventArgs(exception);
            foreach (var handler in this.navigationFailed)
                handler(this, context);
        }
    }
}
