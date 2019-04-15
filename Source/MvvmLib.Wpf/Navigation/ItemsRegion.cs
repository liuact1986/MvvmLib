using MvvmLib.Logger;
using System;
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
        private IItemsRegionAdapter itemsRegionAdapter;
        private SelectableResolver selectableResolver;

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
        /// <param name="contentStrategy">The content strategy</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(IListHistory history, IAnimatedContentStrategy contentStrategy, string regionName, object control)
            : base(contentStrategy, regionName, control)
        {
            this.History = history;
            this.selectableResolver = new SelectableResolver();
            this.itemsRegionAdapter = RegionAdapterContainer.GetItemsRegionAdapter(control.GetType());
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ItemsRegion(string regionName, object control)
            : this(new ListHistory(),
               new AnimatedContentStrategy(), regionName, control)
        { }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.History.List.Count;
        }

        private async Task<bool> ProcessInsertAsync(int index, Type sourceType, object parameter, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            bool navigationSuccess = true;

            try
            {
                var selectedIndex = this.selectableResolver.TrySelect(sourceType, parameter, this.History.ToList());
                if (selectedIndex != -1)
                {
                    if (Control is Selector)
                    {
                        ((Selector)Control).SelectedIndex = selectedIndex;
                    }
                }
                else
                {
                    var currentEntry = this.History.Current;
                    if (currentEntry != null)
                    {
                        this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.Insert);
                    }

                    // create a view for each region container
                    var viewOrObject = this.CreateViewOrObjectInstance(sourceType);
                    if (viewOrObject == null) { throw new InvalidOperationException("View or object null \"" + sourceType.Name + "\""); }

                    var isView = IsView(viewOrObject);
                    var view = viewOrObject as FrameworkElement;// hum ?

                    object context = null;
                    if (isView)
                    {
                        if (view.DataContext != null)
                        {
                            context = view.DataContext;
                        }
                        else
                        {
                            context = ResolveContextWithViewModelLocator(sourceType);
                            if (context != null)
                                view.DataContext = context;
                        }
                    }

                    if (isView)
                    {
                        if (!await CanActivateViewAsync(view, parameter))
                            throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, view, this);
                    }

                    if (context != null)
                    {
                        if (!await CanActivateContextAsync(context, parameter))
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
                    }

                    if (context != null)
                    {
                        this.NotifyRegionKnowledge(this, context);
                    }

                    var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);

                    if (isView)
                    {
                        var listener = new FrameworkElementLoaderListener(view);
                        listener.Subscribe((s, e) =>
                        {
                            var childRegions = FindChildRegions((DependencyObject)s);
                            navigationEntry.ChildRegions = childRegions;
                            listener.Unsubscribe();
                            listener = null;

                            this.NotifyLoadedListener(view, context, parameter);
                        });
                    }

                    // on navigating to
                    if (isView)
                    {
                        OnNavigatingToView(view, parameter);
                        if (context != null)
                            OnNavigatingToContext(context, parameter);
                    }

                    // animate
                    if (isView)
                    {
                        AnimateOnEnter(view, entranceTransitionType, () =>
                        {
                            itemsRegionAdapter.OnInsert(Control, viewOrObject, index);
                        });
                    }
                    else
                    {
                        itemsRegionAdapter.OnInsert(Control, viewOrObject, index);
                    }

                    // history
                    this.History.Insert(index, navigationEntry);

                    // on navigated to
                    if (isView)
                    {
                        OnNavigatedToView(view, parameter);
                        if (context != null)
                            OnNavigatedToContext(context, parameter);
                    }
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
        /// <param name="entranceTransitionType">The entrance navigation type</param>
        public async Task InsertAsync(int index, Type sourceType, object parameter, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.ProcessInsertAsync(index, sourceType, parameter, entranceTransitionType);
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="entranceTransitionType">The entrance navigation type</param>
        public async Task InsertAsync(int index, Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.InsertAsync(index, sourceType, entranceTransitionType);
        }

        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="entranceTransitionType">The entrance navigation type</param>
        /// <returns></returns>
        public async Task AddAsync(Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.AddAsync(sourceType, null, entranceTransitionType);
        }

        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="entranceTransitionType">The entrance navigation type</param>
        public async Task AddAsync(Type sourceType, object parameter, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.InsertAsync(History.List.Count, sourceType, parameter, entranceTransitionType);
        }

        /// <summary>
        /// Remove a view from the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="exitTransitionType">The exit navigation type</param>
        public async Task RemoveAtAsync(int index, ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            try
            {
                var entry = this.History.List[index];
                this.RaiseNavigating(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);

                bool isView = IsView(entry.ViewOrObject);
                FrameworkElement view = entry.ViewOrObject as FrameworkElement;
                object context = entry.Context;

                // can deactivate sub child regions => child regions => current region
                if (entry.ChildRegions.Count > 0)
                {
                    await CheckCanDeactivateChildRegionsAsync(entry.ChildRegions);
                }
                if (isView)
                {
                    if (!await this.CanDeactivateViewAsync(view))
                        throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.View, view, this);
                }
                if (context != null)
                {
                    if (!await CanDeactivateContextAsync(context))
                        throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
                }

                // on navigating from
                this.OnNavigatingFromChildRegions(entry.ChildRegions);

                if (isView)
                {
                    this.OnNavigatingFromView(view);
                }
                if (context != null)
                {
                    this.OnNavigatingFromContext(context);
                }
                if (entry.ChildRegions.Count > 0)
                    this.ClearChildRegions(entry);

                // animate on leave
                if (isView)
                {
                    AnimateOnLeave(view, exitTransitionType, () =>
                    {
                        itemsRegionAdapter.OnRemoveAt(Control, index);
                    });
                }
                else
                {
                    itemsRegionAdapter.OnRemoveAt(Control, index);
                }

                this.History.RemoveAt(index);
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
        /// Remove the last view from the items region.
        /// </summary>
        /// <param name="exitTransitionType">The exit navigation type</param>
        public async Task RemoveLastAsync(ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (this.History.List.Count > 0)
            {
                await this.RemoveAtAsync(this.History.List.Count - 1, exitTransitionType);
            }
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

                await this.RemoveAtAsync(this.History.List.Count - 1);
            }
        }

        internal void ClearItems()
        {
            itemsRegionAdapter.OnClear(Control);
        }
    }
}