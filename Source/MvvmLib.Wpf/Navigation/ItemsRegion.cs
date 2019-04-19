﻿using MvvmLib.Logger;
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
        private readonly IItemsRegionAnimation emptyRegionAnimation;

        private IItemsRegionAdapter itemsRegionAdapter;
        private SelectableResolver selectableResolver;
        private readonly IItemsRegionAnimation defaultRegionNavigationAnimation;

        /// <summary>
        /// The default animation for the region.
        /// </summary>
        public IItemsRegionAnimation DefaultRegionNavigationAnimation
        {
            get { return defaultRegionNavigationAnimation; }
        }
        /// <summary>
        /// Gets the history.
        /// </summary>
        public IListHistory History { get; }

        /// <summary>
        /// Gets the current history entry.
        /// </summary>
        public override NavigationEntry CurrentEntry => this.History.Current;

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="history">The history</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(IListHistory history, string regionName, object control)
            : base(regionName, control)
        {
            this.History = history;
            this.selectableResolver = new SelectableResolver();
            this.itemsRegionAdapter = RegionAdapterContainer.GetItemsRegionAdapter(control.GetType());
            this.defaultRegionNavigationAnimation = new ItemsRegionAnimation(Control, itemsRegionAdapter);
            this.emptyRegionAnimation = new ItemsRegionAnimation(Control, itemsRegionAdapter);

            ((FrameworkElement)control).Unloaded += OnControlUnloaded;
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(string regionName, object control)
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
            ClearRegionAndChildRegions(History.List);
            if (History.Current != null)
                ClearChildRegions(History.Current);
            if(!RegionManager.UnregisterItemsRegion(this))
                this.Logger.Log($"Failed to unregister the items region \"{RegionName}\", control name:\"{ControlName}\"", Category.Debug, Priority.High);
        }

        /// <summary>
        /// Short hand to configure kickly the default animation.
        /// </summary>
        /// <param name="entranceAnimation">The entrance animation</param>
        /// <param name="exitAnimation">The exite animation</param>
        public void ConfigureAnimation(IContentAnimation entranceAnimation, IContentAnimation exitAnimation)
        {
            defaultRegionNavigationAnimation.EntranceAnimation = entranceAnimation;
            defaultRegionNavigationAnimation.ExitAnimation = exitAnimation;
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.History.List.Count;
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

        private async Task<bool> ProcessInsertAsync(int index, Type sourceType, object parameter, IItemsRegionAnimation regionAnimation)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            if (regionAnimation.IsLeaving)
            {
                this.Logger.Log($"Cannot process insert item for the region\"{RegionName}\", the region actually is removing items", Category.Debug, Priority.Medium);
                return false;
            }

            bool navigationSuccess = true;

            try
            {
                var selectedIndex = this.selectableResolver.TrySelect(sourceType, parameter, this.History.ToList());
                if (selectedIndex != -1)
                {
                    if (Control is Selector)
                        ((Selector)Control).SelectedIndex = selectedIndex;
                }
                else
                {
                    var currentEntry = this.History.Current;

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
                    await CheckCanActivateAsync(viewOrObject, context, parameter);

                    // on navigating to
                    OnNavigatingTo(viewOrObject, context, parameter);

                    // insert
                    regionAnimation.DoOnEnter(viewOrObject, index, () => { });

                    // on navigated to
                    OnNavigatedTo(viewOrObject, context, parameter);

                    // history
                    this.History.Insert(index, navigationEntry);

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
            {
                RegionManager.RemoveNonLoadedRegions();
            }

            return navigationSuccess;
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        public async Task InsertAsync(int index, Type sourceType, object parameter)
        {
            await this.ProcessInsertAsync(index, sourceType, parameter, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        public async Task InsertAsync(int index, Type sourceType)
        {
            await this.ProcessInsertAsync(index, sourceType, null, defaultRegionNavigationAnimation);
        }


        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        public async Task AddAsync(Type sourceType, object parameter)
        {
            await this.ProcessInsertAsync(History.List.Count, sourceType, parameter, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <returns></returns>
        public async Task AddAsync(Type sourceType)
        {
            await this.ProcessInsertAsync(History.List.Count, sourceType, null, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Remove a view from the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="regionAnimation">The region animation</param>
        public async Task RemoveAtAsync(int index, IItemsRegionAnimation regionAnimation)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            if (regionAnimation.IsEntering)
            {
                this.Logger.Log($"Cannot process remove item for the region\"{RegionName}\", the region actually is adding items", Category.Debug, Priority.Medium);
                return;
            }
            try
            {
                var entry = this.History.List[index];
                // navigating event
                this.RaiseNavigating(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);

                // Can Deactivate
                await CheckCanDeactivateAsync(entry);

                // on navigating from
                this.OnNavigatingFrom(entry);

                regionAnimation.DoOnLeave(entry.ViewOrObject, index, () => { });

                // history
                this.History.RemoveAt(index);

                // navigated event
                this.RaiseNavigated(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);
            }
            catch (NavigationFailedException ex)
            {
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.RaiseNavigationNavigationFailed(ex);
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                this.Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                this.RaiseNavigationNavigationFailed(navigationFailedException);
            }
        }

        /// <summary>
        /// Remove a view from the items region.
        /// </summary>
        /// <param name="index">The index</param>
        public async Task RemoveAtAsync(int index)
        {
            await this.RemoveAtAsync(index, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Remove the last view from the items region.
        /// </summary>
        public async Task RemoveLastAsync()
        {
            if (this.History.List.Count > 0)
                await this.RemoveAtAsync(this.History.List.Count - 1, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Clear the items region.
        /// </summary>
        public async void Clear()
        {
            while (true)
            {
                if (this.History.List.Count <= 0)
                {
                    break;
                }

                await this.RemoveAtAsync(this.History.List.Count - 1, emptyRegionAnimation);
            }
        }

        internal void ClearItems()
        {
            itemsRegionAdapter.OnClear(Control);
        }
    }
}