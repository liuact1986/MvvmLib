using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Items Region class.
    /// </summary>
    public class ItemsRegion : RegionBase
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

        private IItemsRegionAdapter GetItemsRegionAdapter()
        {
            if (itemsRegionAdapter != null)
            {
                return itemsRegionAdapter;
            }
            else
            {
                itemsRegionAdapter = RegionAdapterContainer.GetItemsRegionAdapter(this.Control.GetType());
                return itemsRegionAdapter;
            }
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

        private bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.History.List.Count;
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
            // index valide ?
            if (IsValidIndex(index))
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
                    var viewOrObject = this.CreateInstance(sourceType);
                    if (viewOrObject == null) { throw new Exception("View or object null \"" + sourceType.Name + "\""); }


                    object context = null;
                    var isView = IsView(viewOrObject);
                    var view = viewOrObject as FrameworkElement;
                    if (isView)
                    {
                        context = GetOrSetViewContext(sourceType, view);
                    }

                    if (await CheckCanActivateAsync(viewOrObject, context, parameter))
                    {
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

                                this.DoLoaded(context, parameter);
                            });
                        }

                        // on navigating to
                        this.DoOnNavigatingTo(viewOrObject, context, parameter);

                        // animate
                        if (isView)
                        {
                            this.contentStrategy.OnEnter(view, () =>
                            {
                                GetItemsRegionAdapter().OnInsert(Control, viewOrObject, index);
                            }, entranceTransitionType);
                        }
                        else
                        {
                            GetItemsRegionAdapter().OnInsert(Control, viewOrObject, index);
                        }

                        // history
                        this.History.Insert(index, navigationEntry);

                        // on navigated to
                        this.DoOnNavigatedTo(viewOrObject, context, parameter);
                        this.RaiseNavigated(sourceType, parameter, RegionNavigationType.Insert);
                    }
                    else
                    {
                        RegionManager.Clean();
                    }
                }
            }
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
        /// Remove a view from the items region.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="exitTransitionType">The exit navigation type</param>
        public async Task RemoveAtAsync(int index, ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (this.IsValidIndex(index))
            {
                var entry = this.History.List[index];
                this.RaiseNavigating(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);

                // deactivate on remove

                // child => parent
                if (await CheckCanDeactivateChildRegionsAsync(entry.ChildRegions)
                    && await this.CheckCanDeactivateAsync(entry.ViewOrObject, entry.Context))
                {
                    this.DoOnNavigatingFromChildRegions(entry.ChildRegions);
                    this.DoOnNavigatingFrom(entry.ViewOrObject, entry.Context);

                    this.DoClearChildRegions(entry);

                    // animate on leave
                    if (IsView(entry.ViewOrObject))
                    {
                        this.contentStrategy.OnLeave((FrameworkElement)entry.ViewOrObject, exitTransitionType, () =>
                        {
                            GetItemsRegionAdapter().OnRemoveAt(Control, index);
                        });
                    }
                    else
                    {
                        GetItemsRegionAdapter().OnRemoveAt(Control, index);
                    }

                    this.History.RemoveAt(index);
                    this.RaiseNavigated(entry.SourceType, entry.Parameter, RegionNavigationType.Remove);
                }
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

                await this.RemoveAtAsync(this.History.List.Count -1);
            }
        }

        internal void ClearItems()
        {
            GetItemsRegionAdapter().OnClear(Control);
        }
    }
}