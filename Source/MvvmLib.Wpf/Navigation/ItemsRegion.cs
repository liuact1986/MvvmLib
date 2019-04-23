using MvvmLib.Logger;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    internal enum ItemsRegionState
    {
        None,
        IsInserting,
        IsRemoving,
    }

    /// <summary>
    /// The Items Region class.
    /// </summary>
    public sealed class ItemsRegion : RegionBase
    {
        private readonly IItemsRegionAdapter itemsRegionAdapter;
        private ItemsRegionState state;

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
        /// Checks if can insert item(s).
        /// </summary>
        public bool CanInsert
        {
            get { return this.state != ItemsRegionState.IsRemoving; }
        }

        /// <summary>
        /// Checks if can remove item(s).
        /// </summary>
        public bool CanRemove
        {
            get { return this.state != ItemsRegionState.IsInserting && this.history.List.Count > 0; }
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="viewOrObjectManager">The view or object manager</param>
        /// <param name="history">The list history</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ItemsRegion(ViewOrObjectManager viewOrObjectManager, IListHistory history, string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : base(viewOrObjectManager, regionName, control, regionsRegistry)
        {
            this.state = ItemsRegionState.None;
            this.history = history;
            this.itemsRegionAdapter = RegionAdapterContainer.GetRegionAdapter(control.GetType());
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ItemsRegion(string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : this(new ViewOrObjectManager(), new ListHistory(), regionName, control, regionsRegistry)
        { }

        /// <summary>
        /// Removes the selectables and singletons from history.
        /// </summary>
        protected override void RemoveSelectablesAndSingletonsFromHistory()
        {
            if (history.Current != null)
            {
                RemoveSelectables(History.Current.SourceType);
                RemoveSingleton(History.Current.SourceType);
            }
            if (history.List.Count > 0)
            {
                RemoveSelectables(History.List);
                RemoveSingletons(History.List);
            }
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

        private bool TrySelect(Type sourceType, object parameter)
        {
            var selectable = viewOrObjectManager.TryGetSelectable(sourceType, parameter);
            if (selectable is FrameworkElement view)
                if (!view.Focus() && view.Parent is UIElement parentElement)
                {
                    parentElement.Focus();
                    return true;
                }
            return false;
        }

        private void DoInsert(object viewOrObject, int index)
        {
            itemsRegionAdapter.OnInsert(control, viewOrObject, index);
        }

        private void DoRemoveAt(int index)
        {
            itemsRegionAdapter.OnRemoveAt(control, index);
        }

        private void TryAddViewOrObject(Type sourceType, object viewOrObject, object context)
        {
            viewOrObjectManager.TryAddViewOrObject(sourceType, viewOrObject, context);
        }

        private async Task<bool> ProcessInsertAsync(int index, Type sourceType, object parameter)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

            bool navigationSuccess = true;

            try
            {
                if (TrySelect(sourceType, parameter))
                    return true;

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
                    HandleViewLoaded(view, (s, e) =>
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
                DoInsert(viewOrObject, index);

                // on navigated to
                OnNavigatedTo(viewOrObject, context, parameter);

                // history
                this.history.Insert(index, navigationEntry);

                TryAddViewOrObject(sourceType, viewOrObject, context);

                // navigated event
                this.RaiseNavigated(sourceType, parameter, RegionNavigationType.Insert);
            }
            catch (NavigationFailedException ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.RaiseNavigationNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
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
            bool success = false;
            if (CanInsert)
            {
                this.state = ItemsRegionState.IsInserting;
                success = await this.ProcessInsertAsync(index, sourceType, parameter);
                this.state = ItemsRegionState.None;
            }
            return success;
        }

        /// <summary>
        /// Insert a view to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The view or object type</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(int index, Type sourceType)
        {
            return await this.InsertAsync(index, sourceType, null);
        }


        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(Type sourceType, object parameter)
        {
            return await this.InsertAsync(history.List.Count, sourceType, parameter);
        }

        /// <summary>
        /// Add a view to the items region.
        /// </summary>
        /// <param name="sourceType">The view or object type</param>
        /// <returns>True if navigation added</returns>
        public async Task<bool> AddAsync(Type sourceType)
        {
            return await this.InsertAsync(history.List.Count, sourceType, null);
        }

        private async Task<bool> ProcessRemoveAtAsync(int index)
        {
            if (!IsValidIndex(index)) { throw new IndexOutOfRangeException(); }

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

                DoRemoveAt(index);

                // history
                this.history.RemoveAt(index);

                // navigated event
                this.RaiseNavigated(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);
            }
            catch (NavigationFailedException ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.RaiseNavigationNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
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
            bool success = false;

            if (CanRemove)
            {
                this.state = ItemsRegionState.IsRemoving;
                success = await this.ProcessRemoveAtAsync(index);
                this.state = ItemsRegionState.None;
            }

            return success;
        }

        /// <summary>
        /// Remove the last view from the items region.
        /// </summary>
        /// <returns>True if navigation removed</returns>
        public async Task<bool> RemoveLastAsync()
        {
            var success = false;

            if (CanRemove)
            {
                this.state = ItemsRegionState.IsRemoving;
                success = await this.ProcessRemoveAtAsync(this.history.List.Count - 1);
                this.state = ItemsRegionState.None;
            }

            return success;
        }

        /// <summary>
        /// Clear the items region.
        /// </summary>
        /// <returns>True if navigation removed</returns>
        public async Task<bool> Clear()
        {
            if (CanRemove)
            {
                var success = true;
                this.state = ItemsRegionState.IsRemoving;

                while (true)
                {
                    if (this.history.List.Count <= 0)
                        break;

                    if (!await this.ProcessRemoveAtAsync(this.history.List.Count - 1))
                        success = false;
                }

                this.state = ItemsRegionState.None;

                return success;
            }

            return false;
        }

        internal void ClearItems()
        {
            itemsRegionAdapter.OnClear(Control);
        }
    }
}