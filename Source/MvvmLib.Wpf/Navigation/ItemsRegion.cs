using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Items Region class.
    /// </summary>
    public sealed class ItemsRegion : RegionBase
    {
        private readonly IItemsRegionNavigationAnimation emptyRegionAnimation;
        private readonly IItemsRegionAdapter itemsRegionAdapter;
        private readonly SelectableResolver selectableResolver;
        private readonly IItemsRegionNavigationAnimation regionNavigation;

        private readonly IListHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public IListHistory History
        {
            get { return history; }
        }

        /// <summary>
        /// Gets the current history entry.
        /// </summary>
        public override NavigationEntry CurrentEntry
        {
            get { return this.history.Current; }
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="history">The history</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(IListHistory history, string regionName, FrameworkElement control)
            : base(regionName, control)
        {
            this.history = history;
            this.selectableResolver = new SelectableResolver();
            this.itemsRegionAdapter = RegionAdapterContainer.GetRegionAdapter(control.GetType());
            this.regionNavigation = new ItemsRegionNavigationAnimation(Control, itemsRegionAdapter);
            this.emptyRegionAnimation = new ItemsRegionNavigationAnimation(Control, itemsRegionAdapter);

            control.Unloaded += OnControlUnloaded;
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(string regionName, FrameworkElement control)
            : this(new ListHistory(), regionName, control)
        { }

        private void ClearRegionAndChildRegions(IEnumerable<NavigationEntry> entries)
        {
            foreach (var entry in entries)
            {
                // sub child => child => parent
                if (entry.ChildRegions.Count > 0)
                    ClearChildRegions(entry);
            }
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            control.Unloaded -= OnControlUnloaded;

            if (!UnregisterItemsRegion(this))
                this.Logger.Log($"Failed to unregister the items region \"{RegionName}\", control name:\"{ControlName}\"", Category.Debug, Priority.High);
        }

        /// <summary>
        /// Short hand to configure kickly the default animation.
        /// </summary>
        /// <param name="entranceAnimation">The entrance animation</param>
        /// <param name="exitAnimation">The exite animation</param>
        public void ConfigureAnimation(IContentAnimation entranceAnimation, IContentAnimation exitAnimation)
        {
            regionNavigation.EntranceAnimation = entranceAnimation;
            regionNavigation.ExitAnimation = exitAnimation;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.history.List.Count;
        }

        private object GetOrSetContext(Type sourceType, object ViewOrObject)
        {
            object context = null;
            var view = ViewOrObject as FrameworkElement;
            if (view != null)
            {
                if (view.DataContext != null)
                    context = view.DataContext;
                else
                {
                    context = ResolveContextWithViewModelLocator(sourceType);
                    if (context != null)
                        view.DataContext = context;
                }
            }
            return context;
        }

        private async Task<bool> ProcessInsertAsync(int index, Type sourceType, object parameter, IItemsRegionNavigationAnimation regionAnimationToUse)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            if (regionAnimationToUse.IsLeaving)
            {
                this.Logger.Log($"Cannot process insert item for the region\"{RegionName}\", the region actually is removing items", Category.Debug, Priority.Medium);
                return false;
            }

            bool navigationSuccess = true;

            try
            {
                var selectedIndex = this.selectableResolver.TrySelect(sourceType, parameter, this.history.ToList());
                if (selectedIndex != -1)
                {
                    if (Control is Selector)
                        ((Selector)Control).SelectedIndex = selectedIndex;
                }
                else
                {
                    var currentEntry = this.history.Current;

                    if (currentEntry != null)
                        // navigating event
                        this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                    // view or object
                    var viewOrObject = this.CreateViewOrObjectInstance(sourceType);
                    if (viewOrObject == null) { throw new ArgumentException($"View or object null \"{sourceType.Name}\""); }

                    // context
                    var context = GetOrSetContext(sourceType, viewOrObject);

                    var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);

                    var view = viewOrObject as FrameworkElement;
                    if (view != null)
                    {
                        // loaded
                        HandleLoaded(view, (s, e) =>
                        {
                            var childRegions = FindChildRegions((DependencyObject)s);
                            navigationEntry.ChildRegions = childRegions;

                            this.NotifyLoadedListeners(view, context, parameter);
                        });
                    }

                    if (context != null)
                        this.NotifyRegionKnowledge(this, context); // ?

                    // Can Activate
                    await CheckCanActivateOrThrowAsync(viewOrObject, context, parameter);

                    // on navigating to
                    OnNavigatingTo(viewOrObject, context, parameter);

                    // insert
                    regionAnimationToUse.DoOnEnter(viewOrObject, index, () => { });

                    // on navigated to
                    OnNavigatedTo(viewOrObject, context, parameter);

                    // history
                    this.history.Insert(index, navigationEntry);

                    // navigated event
                    this.RaiseNavigated(sourceType, parameter, RegionNavigationType.Insert);
                }
            }
            catch (NavigationFailedException ex)
            {
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.RaiseNavigationNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                this.RaiseNavigationNavigationFailed(navigationFailedException);
                navigationSuccess = false;
            }

            if (!navigationSuccess)
                RemoveNonLoadedRegions();

            return navigationSuccess;
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(int index, Type sourceType, object parameter)
        {
            return await this.ProcessInsertAsync(index, sourceType, parameter, regionNavigation);
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(int index, Type sourceType)
        {
            return await this.ProcessInsertAsync(index, sourceType, null, regionNavigation);
        }


        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(Type sourceType, object parameter)
        {
            return await this.ProcessInsertAsync(history.List.Count, sourceType, parameter, regionNavigation);
        }

        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <returns>True if navigation added</returns>
        public async Task<bool> AddAsync(Type sourceType)
        {
            return await this.ProcessInsertAsync(history.List.Count, sourceType, null, regionNavigation);
        }

        private async Task<bool> ProcessRemoveAtAsync(int index, IItemsRegionNavigationAnimation regionNavigationToUse)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            if (regionNavigationToUse.IsEntering)
            {
                this.Logger.Log($"Cannot process remove item for the region\"{RegionName}\", the region actually is adding items", Category.Debug, Priority.Medium);
                return false;
            }

            var navigationSuccess = true;

            try
            {
                var entry = this.history.List[index];
                // navigating event
                this.RaiseNavigating(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);

                // Can Deactivate
                await CheckCanDeactivateOrThrowAsync(entry);

                // on navigating from
                this.OnNavigatingFrom(entry);

                regionNavigationToUse.DoOnLeave(entry.ViewOrObject, index, () => { });

                // history
                this.history.RemoveAt(index);

                // navigated event
                this.RaiseNavigated(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);
            }
            catch (NavigationFailedException ex)
            {
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.RaiseNavigationNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                this.RaiseNavigationNavigationFailed(navigationFailedException);
                navigationSuccess = false;
            }

            return navigationSuccess;
        }

        /// <summary>
        /// Remove a view from the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>True if navigation removed</returns>
        public async Task<bool> RemoveAtAsync(int index)
        {
            return await this.ProcessRemoveAtAsync(index, regionNavigation);
        }

        /// <summary>
        /// Remove the last view from the items region.
        /// </summary>
        /// <returns>True if navigation removed</returns>
        public async Task<bool> RemoveLastAsync()
        {
            if (this.history.List.Count > 0)
                return await this.RemoveAtAsync(this.history.List.Count - 1);

            return false;
        }

        /// <summary>
        /// Clear the items region.
        /// </summary>
        /// <returns>True if navigation removed</returns>
        public async Task<bool> Clear()
        {
            var success = true;
            while (true)
            {
                if (this.history.List.Count <= 0)
                {
                    break;
                }

                if (!await this.ProcessRemoveAtAsync(this.history.List.Count - 1, emptyRegionAnimation))
                    success = false;
            }
            return success;
        }

        internal void ClearItems()
        {
            itemsRegionAdapter.OnClear(Control);
        }
    }
}