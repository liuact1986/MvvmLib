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
        /// Can Deactivate Guard.
        /// </summary>
        protected CanDeactivateGuard canDeactivateGuard;

        /// <summary>
        /// Can Activate Guard.
        /// </summary>
        protected CanActivateGuard canActivateGuard;

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

            this.canDeactivateGuard = new CanDeactivateGuard();
            this.canActivateGuard = new CanActivateGuard();
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

            var currentView = currentEntry.ViewOrObject as FrameworkElement;
            if (currentView != null)
            {
                if (!await canDeactivateGuard.CanDeactivateViewAsync(currentView))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.View, currentView, this);
            }

            var currentContext = currentEntry.Context;
            if (currentContext != null)
            {
                if (!await canDeactivateGuard.CanDeactivateContextAsync(currentContext))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, currentContext, this);
            }
        }


        #endregion // Deactivatable management

        #region IActivatable management

        protected async Task CheckCanActivateAsync(FrameworkElement view, object context, object parameter)
        {
            if (view != null)
                if (!await canActivateGuard.CanActivateViewAsync(view, parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, view, this);

            if (context != null)
                if (!await canActivateGuard.CanActivateContextAsync(context, parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }

        protected async Task CheckCanActivateAsync(NavigationEntry entry)
        {
            // parent => child => sub child
            var view = entry.ViewOrObject as FrameworkElement;
            var parameter = entry.Parameter;
            if (view != null)
                if (!await canActivateGuard.CanActivateViewAsync(view, parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, view, this);

            var context = entry.Context;
            if (context != null)
                if (!await canActivateGuard.CanActivateContextAsync(context, parameter))
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
                    var view = entry.ViewOrObject as FrameworkElement;
                    if (view != null)
                        await childRegion.CheckCanActivateAsync(view, entry.Context, entry.Parameter);


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

            var view = entry.ViewOrObject as FrameworkElement;
            if (view != null)
                NavigationHelper.OnNavigatingFromView(view);

            var context = entry.Context;
            if (context != null)
                NavigationHelper.OnNavigatingFromContext(context);
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

        protected void OnNavigatingTo(FrameworkElement view, object context, object parameter)
        {
            NavigationHelper.OnNavigatingToView(view, parameter);
            if (context != null)
                NavigationHelper.OnNavigatingToContext(context, parameter);
        }

        protected void OnNavigatingTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var view = entry.ViewOrObject as FrameworkElement;
            var parameter = entry.Parameter;
            if (view != null)
                NavigationHelper.OnNavigatingToView(view, parameter);

            var context = entry.Context;
            if (context != null)
                NavigationHelper.OnNavigatingToContext(context, parameter);

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


        protected void OnNavigatedTo(FrameworkElement view, object context, object parameter)
        {
            NavigationHelper.OnNavigatedToView(view, parameter);
            if (context != null)
                NavigationHelper.OnNavigatedToContext(context, parameter);
        }

        protected void OnNavigatedTo(NavigationEntry entry)
        {
            //  parent => child => sub child
            var view = entry.ViewOrObject as FrameworkElement;
            var parameter = entry.Parameter;
            if (view != null)
                NavigationHelper.OnNavigatedToView(view, parameter);

            var context = entry.Context;
            if (context != null)
                NavigationHelper.OnNavigatedToContext(context, parameter);

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
                if (child.GetValue(RegionManager.ContentRegionProperty) != null)
                {
                    // find region
                    var regionName = child.GetValue(RegionManager.ContentRegionProperty) as string;
                    var region = RegionManager.FindContentRegion(regionName, child);
                    if (region != null)
                    {
                        childRegions.Add(region);
                    }
                }
                else if (child.GetValue(RegionManager.ItemsRegionProperty) != null)
                {
                    var regionName = child.GetValue(RegionManager.ItemsRegionProperty) as string;
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
            /*
            clear items child

            regioncontrol
            - 0 viewA
            - 1 ComposedView <=
                -> regionLeft
                - 0 ViewC
                - 1 View x ...
                - 2 View SubChild
                => for each

                -> regionright content region
                    -> SubChildRegion (1) content region
                    -> SubChildRegion (2) content region
                => get child if itemsRegion => clear history / control 
                         if content region => if have child
            - 2 ViewC
            ...
            */

            // sub child => child => parent
            foreach (var child in entry.ChildRegions)
            {
                // sub child
                if (child.CurrentEntry != null && child.CurrentEntry.ChildRegions.Count > 0)
                {
                    child.ClearChildRegions(child.CurrentEntry);
                }

                if (child is ContentRegion)
                {
                    var contentRegion = child as ContentRegion;
                    contentRegion.ClearContent();
                    contentRegion.History.Clear();

                    RegionManager.RemoveContentRegion(contentRegion);
                }
                else
                {
                    var itemsRegion = child as ItemsRegion;
                    itemsRegion.ClearItems();
                    itemsRegion.History.Clear();

                    RegionManager.RemoveItemsRegion(itemsRegion);
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

        protected void NotifyLoadedListener(FrameworkElement view, object context, object parameter)
        {
            if (context != null && context is ILoadedEventListener p)
            {
                p.OnLoaded(view, parameter);
            }
        }

        protected Type GetGenericRegionKnowledge(Type contextType)
        {
            var interfaces = contextType.GetInterfaces();
            foreach (var inter in interfaces)
            {
                if (inter.IsGenericType && inter.GetGenericTypeDefinition() == typeof(IRegionKnowledge<>))
                {
                    var regionType = inter.GetGenericArguments()[0];
                    return regionType;
                }
            }

            return null;
        }

        protected void NotifyRegionKnowledge(IRegion region, object context)
        {
            if (context != null)
            {
                // IRegionKnowledge
                if (context is IRegionKnowledge p)
                {
                    p.GetRegion(region);
                }

                // IRegionKnowledge<T>
                Type contextType = context.GetType();
                var regionType = GetGenericRegionKnowledge(contextType);
                if (regionType != null)
                {
                    var method = contextType.GetMethod("GetRegion", new Type[] { regionType });
                    if (method != null)
                    {
                        if (region.GetType() != regionType)
                        {
                            throw new InvalidOperationException($"Invalid region type. Expected \"{regionType.Name}\", Current \"{region.GetType().Name}\"");
                        }

                        method.Invoke(context, new object[] { region });
                    }
                }
            }
        }

        protected void RaiseNavigating(Type viewType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(viewType, parameter, regionNavigationType);
            foreach (var handler in this.navigating)
            {
                handler(this, context);
            }
        }

        protected void RaiseNavigated(Type viewType, object parameter, RegionNavigationType regionNavigationType)
        {
            var context = new RegionNavigationEventArgs(viewType, parameter, regionNavigationType);
            foreach (var handler in this.navigated)
            {
                handler(this, context);
            }
        }

        protected void RaiseNavigationNavigationFailed(NavigationFailedException exception)
        {
            var context = new RegionNavigationFailedEventArgs(exception);
            foreach (var handler in this.navigationFailed)
            {
                handler(this, context);
            }
        }
    }
}
