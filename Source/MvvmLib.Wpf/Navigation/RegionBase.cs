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
        /// Allows to animate the content of region on enter and leave.
        /// </summary>
        protected IAnimatedContentStrategy animatedContentStrategy;

        /// <summary>
        /// Gets the current entry of history.
        /// </summary>
        public abstract NavigationEntry CurrentEntry { get; }

        /// <summary>
        /// Allows to configure animation.
        /// </summary>
        public IAnimatedContentStrategy Animation => this.animatedContentStrategy;

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
        /// Creates the logger (DebugLogger by default)
        /// </summary>
        /// <returns>The logger to use</returns>
        protected ILogger CreateLogger()
        {
            return new DebugLogger();
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
        /// <param name="contentStrategy"></param>
        /// <param name="regionName"></param>
        /// <param name="control"></param>
        public RegionBase(IAnimatedContentStrategy contentStrategy, string regionName, object control)
        {
            this.animatedContentStrategy = contentStrategy;
            this.RegionName = regionName;
            this.Control = control;
        }

        /// <summary>
        /// Checks if the instance is a FrameworkElement.
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>True or false</returns>
        protected bool IsView(object instance)
        {
            return instance is FrameworkElement;
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

        protected async Task<bool> CanDeactivateViewAsync(FrameworkElement currentView)
        {
            if (currentView is IDeactivatable p)
            {
                var canDeactivate = await p.CanDeactivateAsync();
                return canDeactivate;
            }
            return true;
        }

        protected async Task<bool> CanDeactivateContextAsync(object currentContext)
        {
            if (currentContext is IDeactivatable p)
            {
                var canDeactivate = await p.CanDeactivateAsync();
                return canDeactivate;
            }
            return true;
        }

        protected async Task CheckCanDeactivateChildRegionsAsync(List<RegionBase> childRegions)
        {
            // sub child => child => parent
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child regions
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        await child.CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions);
                    }

                    // current child
                    if (IsView(currentEntry.ViewOrObject))
                    {
                        if (!await child.CanDeactivateViewAsync((FrameworkElement)currentEntry.ViewOrObject))
                        {
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.View, currentEntry.ViewOrObject, this);
                        }
                    }

                    if (currentEntry.Context != null)
                    {
                        if (!await child.CanDeactivateContextAsync(currentEntry.Context))
                        {
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, currentEntry.Context, this);
                        }
                    }
                }
            }
        }

        #endregion // Deactivatable management

        #region IActivatable management

        protected async Task<bool> CanActivateContextAsync(object context, object parameter)
        {
            if (context is IActivatable p)
            {
                var canActivate = await p.CanActivateAsync(parameter);
                return canActivate;
            }
            return true;
        }

        protected async Task<bool> CanActivateViewAsync(FrameworkElement view, object parameter)
        {
            if (view is IActivatable p)
            {
                var canActivate = await p.CanActivateAsync(parameter);
                return canActivate;
            }
            return true;
        }

        protected async Task CheckCanActivateChildRegionsAsync(List<RegionBase> childRegions)
        {
            // parent => child => sub child
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    // current child
                    if (IsView(currentEntry.ViewOrObject))
                    {
                        if (!await child.CanActivateViewAsync((FrameworkElement)currentEntry.ViewOrObject, currentEntry.Parameter))
                        {
                            throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, currentEntry.ViewOrObject, this);
                        }
                    }

                    if (currentEntry.Context != null)
                    {
                        if (!await child.CanActivateContextAsync(currentEntry.Context, currentEntry.Parameter))
                        {
                            throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.Context, currentEntry.Context, this);
                        }
                    }

                    // sub child regions
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        await child.CheckCanActivateChildRegionsAsync(currentEntry.ChildRegions);
                    }
                }
            }
        }

        #endregion // IActivatable management

        #region INavigatable management

        protected void OnNavigatingFromView(FrameworkElement currentView)
        {
            if (currentView is INavigatable p)
            {
                p.OnNavigatingFrom();
            }
        }

        protected void OnNavigatingFromContext(object currentContext)
        {
            if (currentContext is INavigatable p)
            {
                p.OnNavigatingFrom();
            }
        }

        protected void OnNavigatingToView(FrameworkElement view, object parameter)
        {
            if (view is INavigatable p)
            {
                p.OnNavigatingTo(parameter);
            }
        }

        protected void OnNavigatingToContext(object context, object parameter)
        {
            if (context is INavigatable p)
            {
                p.OnNavigatingTo(parameter);
            }
        }

        protected void OnNavigatedToView(FrameworkElement view, object parameter)
        {
            if (view is INavigatable p)
            {
                p.OnNavigatedTo(parameter);
            }
        }

        protected void OnNavigatedToContext(object context, object parameter)
        {
            if (context is INavigatable p)
            {
                p.OnNavigatedTo(parameter);
            }
        }

        protected void OnNavigatingFromChildRegions(List<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var currentEntry = childRegion.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child regions
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        childRegion.OnNavigatingFromChildRegions(currentEntry.ChildRegions);
                    }

                    // current child region
                    if (IsView(currentEntry.ViewOrObject))
                    {
                        childRegion.OnNavigatingFromView((FrameworkElement)currentEntry.ViewOrObject);
                    }
                    if (currentEntry.Context != null)
                    {
                        childRegion.OnNavigatingFromContext(currentEntry.Context);
                    }
                }
            }
        }

        protected void OnNavigatingToChildRegions(List<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var currentEntry = childRegion.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        childRegion.OnNavigatingToChildRegions(currentEntry.ChildRegions);
                    }

                    // current child region
                    if (IsView(currentEntry.ViewOrObject))
                    {
                        childRegion.OnNavigatingToView((FrameworkElement)currentEntry.ViewOrObject, currentEntry.Parameter);
                    }
                    if (currentEntry.Context != null)
                    {
                        childRegion.OnNavigatingToContext(currentEntry.Context, currentEntry.Parameter);
                    }
                }
            }
        }

        protected void OnNavigatedToChildRegions(List<RegionBase> childRegions)
        {
            foreach (var childRegion in childRegions)
            {
                var currentEntry = childRegion.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        childRegion.OnNavigatedToChildRegions(currentEntry.ChildRegions);
                    }

                    // current child region
                    if (IsView(currentEntry.ViewOrObject))
                    {
                        childRegion.OnNavigatedToView((FrameworkElement)currentEntry.ViewOrObject, currentEntry.Parameter);
                    }
                    if (currentEntry.Context != null)
                    {
                        childRegion.OnNavigatedToContext(currentEntry.Context, currentEntry.Parameter);
                    }
                }
            }
        }

        #endregion INavigatable management

        #region Animation Content strategy

        protected void AnimateOnLeave(object content, ExitTransitionType exitTransitionType, Action cb)
        {
            if (content != null && content is FrameworkElement)
            {
                this.animatedContentStrategy.OnLeave((FrameworkElement)content, exitTransitionType, cb);
            }
            else
            {
                cb();
            }
        }

        protected void AnimateOnEnter(object view, EntranceTransitionType entranceTransitionType, Action cb)
        {
            if (view != null && view is FrameworkElement)
            {
                // animate on enter new view
                this.animatedContentStrategy.OnEnter((FrameworkElement)view, () =>
                {
                    cb();
                }, entranceTransitionType);
            }
            else
            {
                cb();
            }
        }

        #endregion // Animation Content strategy

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
