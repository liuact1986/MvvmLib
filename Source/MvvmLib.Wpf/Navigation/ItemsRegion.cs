using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    public class ItemsRegion : RegionBase
    {
        protected IItemsRegionAdapter itemsRegionAdapter;

        protected SelectableResolver selectableResolver;

        public override NavigationEntry CurrentEntry => this.History.Current;

        public IListHistory History { get; }

        public int Count => this.History.List.Count;

        public ItemsRegion(IListHistory history, IAnimatedContentStrategy contentStrategy, string regionName, object control)
            : base(contentStrategy, regionName, control)
        {
            this.History = history;
            this.selectableResolver = new SelectableResolver();
        }

        public ItemsRegion(string regionName, object control)
            : this(new ListHistory(),
               new AnimatedContentStrategy(), regionName, control)
        { }

        protected IItemsRegionAdapter GetItemsRegionAdapter()
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

        public virtual async Task AddAsync(Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.AddAsync(sourceType, null, entranceTransitionType);
        }

        public virtual async Task AddAsync(Type sourceType, object parameter, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.InsertAsync(History.List.Count, sourceType, parameter, entranceTransitionType);
        }

        public async Task InsertAsync(int index, Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
        {
            await this.InsertAsync(index, sourceType, entranceTransitionType);
        }

        public bool IsValidIndex(int index)
        {
            return index >= 0 && index <= this.History.List.Count;
        }

        public virtual async Task InsertAsync(int index, Type sourceType, object parameter, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None)
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


                    // context
                    object context = null;
                    var isView = IsView(viewOrObject);
                    var view = viewOrObject as FrameworkElement;
                    if (isView)
                    {
                        if (view.DataContext != null)
                        {
                            context = view.DataContext;
                        }
                        else
                        {
                            context = ViewModelLocator.GetViewModel(sourceType); // singleton or new instance
                            ViewModelLocator.SetViewModel(view, context); // if(viewModel != null) view.DataContext = viewModel
                        }
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

                                this.DoLoaded(viewOrObject, context, parameter);
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

        public virtual async Task RemoveAtAsync(int index, ExitTransitionType exitTransitionType = ExitTransitionType.None)
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

        public virtual async Task RemoveLastAsync(ExitTransitionType transitionType = ExitTransitionType.None)
        {
            if (this.History.List.Count > 0)
            {
                await this.RemoveAtAsync(this.History.List.Count - 1, transitionType);
            }
        }

        public virtual async void Clear()
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