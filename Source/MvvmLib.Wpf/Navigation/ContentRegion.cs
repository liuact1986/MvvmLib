﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Content Region class.
    /// </summary>
    public class ContentRegion : RegionBase
    {
        private IContentRegionAdapter contentRegionAdapter;

        /// <summary>
        /// The active view or objects.
        /// </summary>
        public Dictionary<Type, object> ActiveViewOrObjects { get; }

        private bool storeActiveViewOrObjects = true;

        /// <summary>
        /// Allows to never store views created.
        /// </summary>
        public bool StoreActiveViewOrObjects
        {
            get { return storeActiveViewOrObjects; }
            set
            {
                storeActiveViewOrObjects = value;
                if (!storeActiveViewOrObjects)
                {
                    ActiveViewOrObjects.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        public INavigationHistory History { get; private set; }

        /// <summary>
        /// Gets the current history entry.
        /// </summary>
        public override NavigationEntry CurrentEntry => this.History.Current;

        /// <summary>
        /// Chekcs if the content region can go back.
        /// </summary>
        public bool CanGoBack => this.History.BackStack.Count > 0;

        /// <summary>
        /// Chekcs if the content region can go forward.
        /// </summary>
        public bool CanGoForward => this.History.ForwardStack.Count > 0;

        private readonly List<EventHandler> canGoBackChanged = new List<EventHandler>();
        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged
        {
            add { if (!canGoBackChanged.Contains(value)) canGoBackChanged.Add(value); }
            remove { if (canGoBackChanged.Contains(value)) canGoBackChanged.Remove(value); }
        }

        private readonly List<EventHandler> canGoForwardChanged = new List<EventHandler>();
        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged
        {
            add { if (!canGoForwardChanged.Contains(value)) canGoForwardChanged.Add(value); }
            remove { if (canGoForwardChanged.Contains(value)) canGoForwardChanged.Remove(value); }
        }

        /// <summary>
        /// Creates an ContentRegion.
        /// </summary>
        /// <param name="history">The history</param>
        /// <param name="contentStrategy">The content strategy</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ContentRegion(INavigationHistory history, IAnimatedContentStrategy contentStrategy, string regionName, object control)
            : base(contentStrategy, regionName, control)
        {
            ActiveViewOrObjects = new Dictionary<Type, object>();
            this.History = history;

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
        }

        /// <summary>
        /// Creates an ContentRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ContentRegion(string regionName, object control)
            : this(new NavigationHistory(), new AnimatedContentStrategy(), regionName, control)
        { }

        private void OnCanGoBackChanged(object sender, EventArgs e)
        {
            foreach (var handler in this.canGoBackChanged)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnCanGoForwardChanged(object sender, EventArgs e)
        {
            foreach (var handler in this.canGoForwardChanged)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private IContentRegionAdapter GetContentRegionAdapter()
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

        private void AddOrUpdateActiveViewOrObject(Type sourceType, object viewOrObject)
        {
            if (StoreActiveViewOrObjects)
            {
                ActiveViewOrObjects[sourceType] = viewOrObject;
            }
        }

        private object GetOrCreateViewOrObject(Type sourceType)
        {
            if (this.ActiveViewOrObjects.TryGetValue(sourceType, out object viewOrObject))
            {
                return viewOrObject;
            }
            else
            {
                viewOrObject = CreateInstance(sourceType);
                AddOrUpdateActiveViewOrObject(sourceType, viewOrObject);
                return viewOrObject;
            }
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

        private void DoOnLeave(object content, ExitTransitionType exitTransitionType, Action cb)
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

        private void DoOnEnter(object view, EntranceTransitionType entranceTransitionType, Action cb)
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

        private async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            bool navigationSuccess = true;

            var currentEntry = this.History.Current;
            if (currentEntry != null)
            {
                this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);
            }

            // can deactivate?
            if (currentEntry == null || (await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions)
                && await CheckCanDeactivateAsync(currentEntry.ViewOrObject, currentEntry.Context)))
            {
                // create a view for each region container
                var viewOrObject = GetOrCreateViewOrObject(sourceType);
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
                        // Called only after "control.Content = view"
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

                    // on navigating from
                    if (currentEntry != null)
                    {
                        this.DoOnNavigatingFromChildRegions(currentEntry.ChildRegions);
                        this.DoOnNavigatingFrom(currentEntry.ViewOrObject, currentEntry.Context);
                    }

                    // on navigating to
                    this.DoOnNavigatingTo(viewOrObject, context, parameter);

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

                    // on navigated to
                    this.DoOnNavigatedTo(viewOrObject, context, parameter);
                    this.RaiseNavigated(sourceType, parameter, RegionNavigationType.New);
                }
                else
                {
                    navigationSuccess = false;
                }
            }
            else
            {
                navigationSuccess = false;
            }

            if (!navigationSuccess)
            {
                RegionManager.Clean();
            }

            return navigationSuccess;
        }

        /// <summary>
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourceType, object parameter,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            await this.ProcessNavigateAsync(sourceType, parameter, entranceTransitionType, exitTransitionType);
        }

        /// <summary>
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourceType, EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            await this.ProcessNavigateAsync(sourceType, null, entranceTransitionType, exitTransitionType);
        }

        private async Task DoSideNavigationAsync(NavigationEntry toGoEntry,
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

                    // on navigating to
                    this.DoOnNavigatingTo(toGoEntry.ViewOrObject, toGoEntry.Context, parameter);

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
                    // on navigated to
                    this.DoOnNavigatedTo(toGoEntry.ViewOrObject, toGoEntry.Context, parameter);
                    this.DoOnNavigatedToChildRegions(toGoEntry.ChildRegions);
                    this.RaiseNavigated(toGoEntry.SourceType, parameter, regionNavigationType); // ?
                }
            }
        }

        /// <summary>
        /// Navigates to previous view.
        /// </summary>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task GoBackAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
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

        /// <summary>
        /// Navigates to next view.
        /// </summary>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task GoForwardAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
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

        /// <summary>
        /// Navigates to the root view.
        /// </summary>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task NavigateToRootAsync(EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
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

        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourceType, object parameter,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            var currentSourceType = this.CurrentEntry?.SourceType;

            // delay 
            await Task.Delay(1);

            if(await this.ProcessNavigateAsync(sourceType, parameter, entranceTransitionType, exitTransitionType))
            {
                if (currentSourceType != null)
                {
                    // remove page from history
                    var entry = this.History.Previous;
                    if (entry != null && entry.SourceType == currentSourceType)
                    {
                        this.History.BackStack.Remove(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <param name="entranceTransitionType">The entrance transition type</param>
        /// <param name="exitTransitionType">The exit transition type</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourceType,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            await RedirectAsync(sourceType, null, entranceTransitionType, exitTransitionType);
        }

    }
}
