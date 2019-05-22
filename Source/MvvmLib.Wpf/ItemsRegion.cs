using MvvmLib.Animation;
using MvvmLib.Logger;
using MvvmLib.Utils;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The Items Region class.
    /// </summary>
    public sealed class ItemsRegion : RegionBase
    {
        private readonly IItemsRegionAdapter regionAdapter;
        /// <summary>
        /// The items region adapter.
        /// </summary>
        public IItemsRegionAdapter RegionAdapter
        {
            get { return regionAdapter; }
        }

        private readonly ItemsRegionHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public ItemsRegionHistory History
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

        private SynchronizationHandling synchronizationMode;
        /// <summary>
        /// The Synchronization mode
        /// </summary>
        public SynchronizationHandling SynchronizationMode
        {
            get { return synchronizationMode; }
            set { synchronizationMode = value; }
        }

        /// <summary>
        /// Creates an ItemsRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ItemsRegion(string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : base(regionName, control, regionsRegistry)
        {
            this.history = new ItemsRegionHistory();
            this.synchronizationMode = SynchronizationHandling.SynchronizeControlWithCurrentIndexOfHistory;

            this.regionAdapter = RegionAdapterResolver.GetRegionAdapter(control);
            this.regionAdapter.Adapt(this);
        }

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.history.Entries.Count;
        }

        private bool TrySelectExisting(Type sourceType, object parameter)
        {
            var selectableRegistration = TryGetSelectable(sourceType, parameter);
            if (selectableRegistration != null)
            {
                if (control is Selector selector)
                {
                    selector.SelectedItem = selectableRegistration.ViewOrObject;
                    return true;
                }
                else if (selectableRegistration.IsView)
                {
                    var view = selectableRegistration.ViewOrObject as FrameworkElement;
                    if (view != null && view.Parent is UIElement parentElement)
                    {
                        parentElement.Focus();
                        return true;
                    }
                }
                else
                {
                    if (control is ItemsControl itemsControl)
                    {
                        var items = itemsControl.Items;
                        foreach (var item in items)
                        {
                            if (item is FrameworkElement frameworkElement)
                            {
                                if (frameworkElement.DataContext != null && frameworkElement.DataContext.GetType() == sourceType)
                                {
                                    frameworkElement.Focus();
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Finds the index of the control with the item type. The item type is used to find the parent control from child element.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="element">The child element</param>
        /// <returns>The index or -1</returns>
        public int FindControlIndex<T>(UIElement element) where T : DependencyObject
        {
            if (control is ItemsControl itemsControl)
            {
                var target = XamlHelper.FindParent<T>(element);
                if (target is FrameworkElement)
                    return EnumerableUtils.FindIndex(itemsControl.Items, target);
            }
            else if (control is TransitioningItemsControl transitioningItemsControl)
            {
                return transitioningItemsControl.FindControlIndex(element);
            }
            else
            {
                throw new NotSupportedException($"FindControlIndex not supported for {control.GetType().Name}. Using XamlHelper directly is an alternative.");
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the view model with the item type. The item type is used to find the control from child element.
        /// </summary>
        /// <typeparam name="T">The item type</typeparam>
        /// <param name="element">The child element</param>
        /// <returns>The index or -1</returns>
        public int FindContextIndex<T>(UIElement element) where T : DependencyObject
        {
            if (control is ItemsControl itemsControl)
            {
                var target = XamlHelper.FindParent<T>(element);
                if (target is FrameworkElement frameworkElement)
                {
                    var context = frameworkElement.DataContext;
                    if (context != null)
                        return EnumerableUtils.FindIndex(itemsControl.Items, context);
                }
            }
            else if (control is TransitioningItemsControl transitioningItemsControl)
            {
                return transitioningItemsControl.FindContextIndex(element);
            }
            else
            {
                throw new NotSupportedException($"FindContextIndex not supported for {control.GetType().Name}. Using XamlHelper directly is an alternative.");
            }

            return -1;
        }

        /// <summary>
        /// Finds the index of the item (view or view model).
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns></returns>
        public int FindIndex(object item)
        {
            int index = 0;
            foreach (var entry in history.Entries)
            {
                if (entry.Source == item || entry.Context == item)
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        private async Task<bool> ProcessInsertAsync(int index, Type sourceType, object parameter)
        {
            bool navigationSuccess = true;

            try
            {
                if (!IsValidIndex(index))
                    throw new IndexOutOfRangeException();

                if (TrySelectExisting(sourceType, parameter))
                    return true;

                var currentEntry = this.history.Current;

                if (currentEntry != null)
                    // navigating event
                    this.OnNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                // view or object
                var viewOrObject = CreateInstance(sourceType);
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
                        navigationEntry.childRegions = childRegions;

                        this.NotifyLoadedListeners(viewOrObject, context, parameter);
                    });
                }

                if (context != null)
                    this.NotifyRegionKnowledge(this, context); // ?

                // Can Activate
                await CheckCanActivateOrThrowAsync(viewOrObject, context, parameter);

                // on navigating to
                OnNavigatingTo(viewOrObject, context, parameter);

                // history
                this.history.Insert(index, navigationEntry);

                if (view == null)
                {
                    await Task.Delay(1);

                    var childRegions = FindChildRegions(control);
                    navigationEntry.childRegions = childRegions;

                    this.NotifyLoadedListeners(viewOrObject, context, parameter);
                }

                // on navigated to
                OnNavigatedTo(viewOrObject, context, parameter);

                TryAddSelectable(sourceType, viewOrObject, context);

                // navigated event
                this.OnNavigated(sourceType, parameter, RegionNavigationType.Insert);
            }
            catch (NavigationFailedException ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.OnNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                this.OnNavigationFailed(navigationFailedException);
                navigationSuccess = false;
            }

            return navigationSuccess;
        }

        /// <summary>
        /// Inserts a source to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(int index, Type sourceType, object parameter)
        {
            return await this.ProcessInsertAsync(index, sourceType, parameter);
        }

        /// <summary>
        /// Inserts a source to the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if inserted</returns>
        public async Task<bool> InsertAsync(int index, Type sourceType)
        {
            return await this.InsertAsync(index, sourceType, null);
        }

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(Type sourceType, object parameter)
        {
            return await this.InsertAsync(history.Entries.Count, sourceType, parameter);
        }

        /// <summary>
        /// Adds a source to the items region.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>True if added</returns>
        public async Task<bool> AddAsync(Type sourceType)
        {
            return await this.InsertAsync(history.Entries.Count, sourceType, null);
        }

        private async Task<bool> ProcessRemoveAtAsync(int index)
        {
            var navigationSuccess = true;

            try
            {
                if (!IsValidIndex(index))
                    throw new IndexOutOfRangeException();

                var entry = this.history.Entries[index];
                // navigating event
                this.OnNavigating(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);

                // Can Deactivate
                await CheckCanDeactivateOrThrowAsync(entry);

                // on navigating from
                this.OnNavigatingFrom(entry);

                // history
                this.history.RemoveAt(index);

                RemoveSelectable(entry);

                // navigated event
                this.OnNavigated(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);
            }
            catch (NavigationFailedException ex)
            {
                Logger.Log(ex.ToString(), Category.Exception, Priority.Low);
                this.OnNavigationFailed(ex);
                navigationSuccess = false;
            }
            catch (Exception ex)
            {
                var navigationFailedException = new NavigationFailedException(NavigationFailedExceptionType.ExceptionThrown, NavigationFailedSourceType.InnerException, ex, this);
                Logger.Log(ex.ToString(), Category.Exception, Priority.High);
                this.OnNavigationFailed(navigationFailedException);
                navigationSuccess = false;
            }

            return navigationSuccess;
        }

        /// <summary>
        /// Removes the entry from the items region history.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAtAsync(int index)
        {
            return await this.ProcessRemoveAtAsync(index);
        }

        /// <summary>
        /// Tries to find the source and remove the entry from history.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>True if removed</returns>
        public async Task<bool> RemoveAsync(object source)
        {
            var index = FindIndex(source);
            if (index != -1)
            {
                return await this.ProcessRemoveAtAsync(index);
            }
            return false;
        }

        /// <summary>
        /// Clears the region history.
        /// </summary>
        /// <returns>True if success</returns>
        public async Task<bool> Clear()
        {
            var success = true;
            while (true)
            {
                if (this.history.Entries.Count <= 0)
                    break;

                if (!await this.ProcessRemoveAtAsync(this.history.Entries.Count - 1))
                    success = false;
            }
            return success;
        }

        internal void ClearItems()
        {
            history.Clear();
        }
    }

    /// <summary>
    /// The Synchronization mode for Current index changed event.
    /// </summary>
    public enum SynchronizationHandling
    {
        /// <summary>
        /// Synchronize the control (Selector) withhen the history current index 
        /// </summary>
        SynchronizeControlWithCurrentIndexOfHistory,
        /// <summary>
        /// No synchronization
        /// </summary>
        None
    }
}