using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{

    public class ContentRegion : RegionBase
    {
        public Dictionary<Type, object> ActiveViewOrObjects { get; } 

        public INavigationHistory History { get; protected set; }

        protected IContentRegionAdapter contentRegionAdapter;

        public bool CanGoBack => this.History.BackStack.Count > 0;

        public bool CanGoForward => this.History.ForwardStack.Count > 0;

        public override NavigationEntry CurrentEntry => this.History.Current;

        public ContentRegion(INavigationHistory history, IAnimatedContentStrategy contentStrategy, string regionName, object control)
            : base(contentStrategy, regionName, control)
        {
            ActiveViewOrObjects  = new Dictionary<Type, object>();
            this.History = history;
        }

        public ContentRegion(string regionName, object control)
            : this(new NavigationHistory(), new AnimatedContentStrategy(), regionName, control)
        { }

        protected IContentRegionAdapter GetContentRegionAdapter()
        {
            if (contentRegionAdapter != null)
            {
                return contentRegionAdapter;
            }
            else
            {
                contentRegionAdapter = RegionAdapterContainer.GetContentRegionAdapter(this.Control.GetType());
                return contentRegionAdapter;
            }
        }

        protected bool IsActiveViewOrObjectRegistered(Type sourceType)
        {
            return ActiveViewOrObjects.ContainsKey(sourceType);
        }

        protected void AddOrUpdateActiveViewOrObject(Type sourceType, object viewOrObject)
        {
            ActiveViewOrObjects[sourceType] = viewOrObject;
        }

        internal void SetContent(FrameworkElement view,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            var content = GetContentRegionAdapter().GetContent(Control);
            this.contentStrategy.OnLeave((FrameworkElement)content, exitTransitionType, () =>
            {
                // animate on enter new view
                this.contentStrategy.OnEnter(view, () =>
                {
                    // set content
                    GetContentRegionAdapter().OnNavigate(Control, view);
                }, entranceTransitionType);
            });
        }

        protected virtual void DoOnLeave(object content, ExitTransitionType exitTransitionType, Action cb)
        {
            if (content != null && content is FrameworkElement)
            {
                this.contentStrategy.OnLeave((FrameworkElement)content, exitTransitionType, cb);
            }
            else
            {
                cb();
            }
        }

        protected virtual void DoOnEnter(object view, EntranceTransitionType entranceTransitionType, Action cb)
        {
            if (view != null && view is FrameworkElement)
            {
                // animate on enter new view
                this.contentStrategy.OnEnter((FrameworkElement)view, () =>
                {
                    cb();
                }, entranceTransitionType);
            }
            else
            {
                cb();
            }
        }

        public virtual async Task NavigateAsync(Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            await this.NavigateAsync(sourceType, null, entranceTransitionType, exitTransitionType);
        }

        public virtual async Task NavigateAsync(Type sourceType, object parameter,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            bool navigationFailed = false;

            var currentEntry = this.History.Current;
            if (currentEntry != null)
            {
                this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);
            }

            // can deactivate?
            if (currentEntry == null || (await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions)
                && await CheckCanDeactivateAsync(currentEntry.ViewOrObject, currentEntry.Context)))
            {

                var isActiveViewOrObjectRegistered = IsActiveViewOrObjectRegistered(sourceType);

                // create a view for each region container
                var viewOrObject = isActiveViewOrObjectRegistered ? ActiveViewOrObjects[sourceType] : this.CreateInstance(sourceType);
                if (viewOrObject == null) { throw new Exception("View or object null \"" + sourceType.Name + "\""); }

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
                        // Called only after "control.Content = view"
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

                    // on navigating from
                    if (currentEntry != null)
                    {
                        this.DoOnNavigatingFromChildRegions(currentEntry.ChildRegions);
                        this.DoOnNavigatingFrom(currentEntry.ViewOrObject, currentEntry.Context);
                    }
                    // on navigated to
                    this.DoOnNavigatedTo(viewOrObject, context, parameter);

                    // animate
                    var content = GetContentRegionAdapter().GetContent(Control);
                    this.DoOnLeave(content, exitTransitionType, () =>
                    {
                        this.DoOnEnter(view, entranceTransitionType, () =>
                        {
                            // set content
                            GetContentRegionAdapter().OnNavigate(Control, viewOrObject);
                        });
                    });

                    // history
                    AddOrUpdateActiveViewOrObject(sourceType, viewOrObject);
                    this.History.Navigate(navigationEntry);
                    this.RaiseNavigated(sourceType, parameter, RegionNavigationType.New);
                }
                else
                {
                    navigationFailed = true;
                }
            }
            else
            {
                navigationFailed = true;
            }

            if (navigationFailed)
            {
                RegionManager.Clean();
            }
        }

        protected virtual async Task DoSideNavigationAsync(NavigationEntry toGoEntry,
           RegionNavigationType regionNavigationType,
           Action<object, object> setContentCallback,
           Action onCompleteCallback,
           EntranceTransitionType entranceTransitionType,
           ExitTransitionType exitTransitionType)
        {
            var currentEntry = this.History.Current;
            this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.Back);

            var parameter = toGoEntry.Parameter;
            if (await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions)
                && await CheckCanDeactivateAsync(currentEntry.ViewOrObject, currentEntry.Context))
            {
                // parent => child => sub child
                if (await CheckCanActivateAsync(toGoEntry.ViewOrObject, toGoEntry.Context, parameter)
                    && await CheckCanActivateChildRegionsAsync(toGoEntry.ChildRegions))
                {
                    // on navigating from
                    this.DoOnNavigatingFromChildRegions(currentEntry.ChildRegions);
                    this.DoOnNavigatingFrom(currentEntry.ViewOrObject, currentEntry.Context);

                    // on navigated to
                    this.DoOnNavigatedTo(toGoEntry.ViewOrObject, toGoEntry.Context, parameter);
                    this.DoOnNavigatedToChildRegions(toGoEntry.ChildRegions);

                    // animate
                    var content = GetContentRegionAdapter().GetContent(Control);
                    this.DoOnLeave(content, exitTransitionType, () =>
                    {
                        this.DoOnEnter(toGoEntry.ViewOrObject, entranceTransitionType, () =>
                        {
                            setContentCallback(Control, toGoEntry.ViewOrObject);
                        });
                    });

                    onCompleteCallback();
                    this.RaiseNavigated(toGoEntry.SourceType, parameter, regionNavigationType); // ?
                }
            }
        }

        public virtual async Task GoBackAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
          ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (this.CanGoBack)
            {
                await this.DoSideNavigationAsync(History.Previous, RegionNavigationType.Back,
                    (control, view) => GetContentRegionAdapter().OnGoBack(control, view),
                    () => History.GoBack(),
                    entranceTransitionType,
                    exitTransitionType);
            }
        }

        public virtual async Task GoForwardAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (this.CanGoForward)
            {
                await this.DoSideNavigationAsync(History.Next, RegionNavigationType.Forward,
                   (control, view) => GetContentRegionAdapter().OnGoForward(control, view),
                   () => History.GoForward(),
                   entranceTransitionType,
                   exitTransitionType);
            }
        }

        public virtual async Task NavigateToRootAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
          ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (this.CanGoBack)
            {
                await this.DoSideNavigationAsync(History.Root, RegionNavigationType.Root,
                  (control, view) => GetContentRegionAdapter().OnNavigate(control, view),
                  () => History.NavigateToRoot(),
                  entranceTransitionType,
                  exitTransitionType);
            }
        }

    }
}
