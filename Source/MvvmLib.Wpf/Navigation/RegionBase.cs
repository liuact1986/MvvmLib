using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MvvmLib.Navigation
{
    public abstract class RegionBase : IRegion
    {
        public string RegionName { get; protected set; }

        public object Control { get; }

        public string Name => ((FrameworkElement)Control).Name;

        public bool IsLoaded { get; internal set; }

        protected IAnimatedContentStrategy contentStrategy;

        protected NavigationGuard guard;

        public abstract NavigationEntry CurrentEntry { get; }

        public IAnimatedContentStrategy Animation => this.contentStrategy;

        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigating = new List<EventHandler<RegionNavigationEventArgs>>();
        public event EventHandler<RegionNavigationEventArgs> Navigating
        {
            add { if (!navigating.Contains(value)) navigating.Add(value); }
            remove { if (navigating.Contains(value)) navigating.Remove(value); }
        }

        protected readonly List<EventHandler<RegionNavigationEventArgs>> navigated = new List<EventHandler<RegionNavigationEventArgs>>();
        public event EventHandler<RegionNavigationEventArgs> Navigated
        {
            add { if (!navigated.Contains(value)) navigated.Add(value); }
            remove { if (navigated.Contains(value)) navigated.Remove(value); }
        }

        protected readonly List<EventHandler<RegionNavigationFailedEventArgs>> navigationFailed = new List<EventHandler<RegionNavigationFailedEventArgs>>();
        public event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed
        {
            add { if (!navigationFailed.Contains(value)) navigationFailed.Add(value); }
            remove { if (navigationFailed.Contains(value)) navigationFailed.Remove(value); }
        }

        public RegionBase(IAnimatedContentStrategy contentStrategy, string regionName, object control)
        {
            this.contentStrategy = contentStrategy;
            this.guard = new NavigationGuard();
            this.guard.SetCancellationCallback(OnActivationCancel, OnDeactivationCancel);

            this.RegionName = regionName;
            this.Control = control;
        }

        protected bool IsView(object instance)
        {
            return instance is FrameworkElement;
        }

        protected virtual object CreateInstance(Type viewType)
        {
            return ViewResolver.Resolve(viewType);
        }

        protected virtual object GetOrSetViewContext(Type sourceType, FrameworkElement view)
        {
            object context = null;
            if (view.DataContext != null)
            {
                context = view.DataContext;
            }
            else
            {
                var viewModelType = ViewModelLocationProvider.ResolveViewModelType(sourceType); // singleton or new instance
                if (viewModelType != null)
                {
                    context = ViewModelLocationProvider.ResolveViewModel(viewModelType);
                    view.DataContext = context;
                }
            }
            return context;
        }

        protected virtual async Task<bool> CheckCanDeactivateAsync(object view, object context)
        {
            var canDeactivateView = view is IDeactivatable ? await this.guard.CheckCanDeactivateAsync((IDeactivatable)view) : true;
            if (!canDeactivateView)
            {
                return false;
            }
            var canDeactivateViewModel = context != null && context is IDeactivatable ?
                await this.guard.CheckCanDeactivateAsync((IDeactivatable)context)
                : true;
            return canDeactivateViewModel;
        }

        protected virtual async Task<bool> CheckCanActivateAsync(object viewOrObject, object context, object parameter)
        {
            var canActivateView = viewOrObject is IActivatable ?
                await this.guard.CheckCanActivateAsync((IActivatable)viewOrObject, parameter)
                : true;
            if (!canActivateView)
            {
                return false;
            }

            var canActivateViewModel = context != null && context is IActivatable ?
                await this.guard.CheckCanActivateAsync((IActivatable)context, parameter)
                : true;
            return canActivateViewModel;
        }

        protected virtual void DoOnNavigatingFrom(object view, object context)
        {
            if (view is INavigatable)
            {
                ((INavigatable)view).OnNavigatingFrom();
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatingFrom();
            }
        }

        protected virtual void DoOnNavigatingTo(object view, object context, object parameter)
        {
            if (view is INavigatable)
            {
                ((INavigatable)view).OnNavigatingTo(parameter);
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatingTo(parameter);
            }
        }

        protected virtual void DoOnNavigatedTo(object view, object context, object parameter)
        {
            if (view is INavigatable)
            {
                ((INavigatable)view).OnNavigatedTo(parameter);
            }
            if (context != null && context is INavigatable)
            {
                ((INavigatable)context).OnNavigatedTo(parameter);
            }
        }

        protected virtual List<RegionBase> FindChildRegions(DependencyObject parent)
        {
            var result = new List<RegionBase>();

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
                        result.Add(region);
                    }
                }
                else if (child.GetValue(RegionManager.ItemsRegionProperty) != null)
                {
                    var regionName = child.GetValue(RegionManager.ItemsRegionProperty) as string;
                    var region = RegionManager.FindItemsRegion(regionName, child);
                    if (region != null)
                    {
                        result.Add(region);
                    }
                }
                result.AddRange(FindChildRegions(child));
            }
            return result;

        }

        protected void DoClearChildRegions(NavigationEntry entry)
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
            if (entry.ChildRegions.Count > 0)
            {
                foreach (var child in entry.ChildRegions)
                {
                    // sub child
                    if (child.CurrentEntry != null && child.CurrentEntry.ChildRegions.Count > 0)
                    {
                        child.DoClearChildRegions(child.CurrentEntry);
                    }

                    if (child is ContentRegion)
                    {
                        var contentRegion = child as ContentRegion;
                        contentRegion.SetContent(null);
                        contentRegion.History.Clear();

                        RegionManager.contentRegions[contentRegion.RegionName].Remove(contentRegion);
                    }
                    else
                    {
                        var itemsRegion = child as ItemsRegion;
                        itemsRegion.ClearItems();
                        itemsRegion.History.Clear();

                        RegionManager.itemsRegions[itemsRegion.RegionName].Remove(itemsRegion);
                    }
                }
            }
        }

        protected async Task<bool> CheckCanDeactivateChildRegionsAsync(List<RegionBase> childRegions)
        {
            // sub child => child => parent
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    var canDeactivateSubChild = currentEntry.ChildRegions.Count > 0 ?
                         await child.CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions)
                         : true;
                    if (!canDeactivateSubChild)
                    {
                        return false;
                    }
                    if (!await child.CheckCanDeactivateAsync(currentEntry.ViewOrObject, currentEntry.Context))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        protected virtual void DoOnNavigatingFromChildRegions(List<RegionBase> childRegions)
        {
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        child.DoOnNavigatingFromChildRegions(currentEntry.ChildRegions);
                    }

                    // child
                    child.DoOnNavigatingFrom(currentEntry.ViewOrObject, currentEntry.Context);
                }
            }
        }

        protected virtual void DoOnNavigatedToChildRegions(List<RegionBase> childRegions)
        {
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    // sub child
                    if (currentEntry.ChildRegions.Count > 0)
                    {
                        child.DoOnNavigatedToChildRegions(currentEntry.ChildRegions);
                    }

                    // child
                    child.DoOnNavigatedTo(currentEntry.ViewOrObject, currentEntry.Context, currentEntry.Parameter);
                }
            }
        }

        protected async Task<bool> CheckCanActivateChildRegionsAsync(List<RegionBase> childRegions)
        {
            // parent => child => sub child
            foreach (var child in childRegions)
            {
                var currentEntry = child.CurrentEntry;
                if (currentEntry != null)
                {
                    if (!await child.CheckCanActivateAsync(currentEntry.ViewOrObject, currentEntry.Context, currentEntry.Parameter))
                    {
                        return false;
                    }

                    var canActivateSubChild = currentEntry.ChildRegions.Count > 0 ?
                         await child.CheckCanActivateChildRegionsAsync(currentEntry.ChildRegions)
                         : true;
                    if (!canActivateSubChild)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        protected virtual void OnDeactivationCancel(IDeactivatable source)
        {
            RaiseNavigationCancelled(source);
        }

        protected virtual void OnActivationCancel(IActivatable source, object parameter)
        {
            RaiseNavigationCancelled(source, parameter);
        }

        protected virtual void DoLoaded(object context, object parameter)
        {
            if (context != null && context is ILoadedEventListener)
            {
                ((ILoadedEventListener)context).OnLoaded(parameter);
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

        protected void RaiseNavigationCancelled(object source, object parameter = null)
        {
            var context = new RegionNavigationFailedEventArgs(source, parameter);
            foreach (var handler in this.navigationFailed)
            {
                handler(this, context);
            }
        }
    }
}
