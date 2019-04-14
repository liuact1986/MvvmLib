using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The Content Region class.
    /// </summary>
    public sealed class ContentRegion : RegionBase
    {
        private readonly IContentRegionAdapter contentRegionAdapter;

        private readonly Dictionary<Type, object> viewOrObjectsSingletons;

        private readonly Dictionary<Type, List<KeyValuePair<object, object>>> activeViewOrObjects;

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
            viewOrObjectsSingletons = new Dictionary<Type, object>();
            activeViewOrObjects = new Dictionary<Type, List<KeyValuePair<object, object>>>();
            this.History = history;
            this.contentRegionAdapter = RegionAdapterContainer.GetContentRegionAdapter(control.GetType());

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

        #region View or object management

        private void AddActiveViewOrObject(Type sourceType, object viewOrObject, object context)
        {
            bool isSingleton = false;
            // singleton 
            if (viewOrObject is IViewLifetimeStrategy p)
            {
                if (p.Strategy == StrategyType.Singleton)
                {
                    isSingleton = true;
                    viewOrObjectsSingletons[sourceType] = viewOrObject;
                }
            }
            else if (context != null && context is IViewLifetimeStrategy p2)
            {
                if (p2.Strategy == StrategyType.Singleton)
                {
                    isSingleton = true;
                    viewOrObjectsSingletons[sourceType] = viewOrObject;
                }
            }

            if (!isSingleton)
            {
                if (!activeViewOrObjects.ContainsKey(sourceType))
                {
                    activeViewOrObjects[sourceType] = new List<KeyValuePair<object, object>>();
                }
                activeViewOrObjects[sourceType].Add(new KeyValuePair<object, object>(viewOrObject, context));
            }
        }


        private object TryGetExistingViewOrObjectSingleton(Type sourceType)
        {
            if (viewOrObjectsSingletons.TryGetValue(sourceType, out object viewSingleton))
            {
                return viewSingleton;
            }
            return null;
        }

        private object TryGetExistingViewOrObject(Type sourceType, object parameter)
        {
            if (activeViewOrObjects.TryGetValue(sourceType, out List<KeyValuePair<object, object>> instances))
            {
                foreach (var instance in instances)
                {
                    var context = instance.Value;
                    if (context != null && context is ISelectable p)
                    {
                        if (p.IsTarget(sourceType, parameter))
                        {
                            // view
                            return instance.Key;
                        }
                    }
                }
            }
            return null;
        }

        private ViewOrObjectInstanceResult GetOrCreateViewOrObjectInstance(Type sourceType, object parameter)
        {
            var singletonViewOrObject = TryGetExistingViewOrObjectSingleton(sourceType);
            if (singletonViewOrObject != null)
            {
                return new ViewOrObjectInstanceResult(ResolutionType.Singleton, singletonViewOrObject);
            }

            var existingViewOrObject = TryGetExistingViewOrObject(sourceType, parameter);
            if (existingViewOrObject != null)
            {
                return new ViewOrObjectInstanceResult(ResolutionType.Existing, existingViewOrObject);
            }

            var newViewOrObject = CreateViewOrObjectInstance(sourceType);
            return new ViewOrObjectInstanceResult(ResolutionType.New, newViewOrObject);
        }

        #endregion // View or object management


        /// <summary>
        /// Clear content with no animation.
        /// </summary>
        public void ClearContent()
        {
            contentRegionAdapter.OnNavigate(Control, null);
        }

        private async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter,
            EntranceTransitionType entranceTransitionType = EntranceTransitionType.None,
            ExitTransitionType exitTransitionType = ExitTransitionType.None)
        {
            if (sourceType == null) { throw new ArgumentNullException(nameof(sourceType)); }

            bool navigationSuccess = true;

            try
            {
                var currentEntry = this.History.Current;
                var hasCurrentEntry = currentEntry != null;
                bool currentIsView = false;
                FrameworkElement currentView = null;
                object currentContext = null;
                if (hasCurrentEntry)
                {
                    this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                    currentIsView = IsView(currentEntry.ViewOrObject);
                    if (currentIsView)
                    {
                        currentView = currentEntry.ViewOrObject as FrameworkElement;
                    }
                    currentContext = currentEntry.Context;
                }

                // Can deactivate current
                if (hasCurrentEntry)
                {
                    // throw a navigation failed exception on fail
                    await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions);

                    if (currentIsView)
                    {
                        if (!await CanDeactivateViewAsync(currentView))
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.View, currentView, this);
                    }

                    if (currentContext != null)
                    {
                        if (!await CanDeactivateContextAsync(currentContext))
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, currentContext, this);
                    }
                }

                // view and context
                var viewOrObjectResult = GetOrCreateViewOrObjectInstance(sourceType, parameter);
                object viewOrObject = viewOrObjectResult.Instance;

                var isView = IsView(viewOrObject);
                var view = viewOrObject as FrameworkElement; // hum ?
                object context = null;
                if (viewOrObjectResult.ResolutionType == ResolutionType.New)
                {
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
                }
                else
                {
                    if (isView)
                        if (view.DataContext != null)
                            context = view.DataContext;
                }

                var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);

                if (viewOrObjectResult.ResolutionType == ResolutionType.New && isView)
                {
                    // Called only after "control.Content = view"
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

                // can activate new 
                if (isView)
                {
                    if (!await CanActivateViewAsync(view, parameter))
                        throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, view, this);

                    if (context != null)
                    {
                        if (!await CanActivateContextAsync(context, parameter))
                            throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
                    }
                }

                if (context != null)
                {
                    this.NotifyRegionKnowledge(this, context);
                }

                // on navigating from
                if (hasCurrentEntry)
                {
                    if (currentIsView)
                    {
                        OnNavigatingFromChildRegions(currentEntry.ChildRegions);
                        OnNavigatingFromView(currentView);
                    }
                    if (currentContext != null)
                        OnNavigatingFromContext(currentContext);

                    if (currentEntry.ChildRegions.Count > 0)
                        this.ClearChildRegions(currentEntry);
                }

                if (viewOrObjectResult.ResolutionType == ResolutionType.New || viewOrObjectResult.ResolutionType == ResolutionType.Singleton)
                {
                    // on navigating to
                    if (isView)
                    {
                        OnNavigatingToView(view, parameter);
                        if (context != null)
                            OnNavigatingToContext(context, parameter);
                    }
                }

                // change content
                var currentRegionContent = contentRegionAdapter.GetContent(Control);
                this.AnimateOnLeave(currentRegionContent, exitTransitionType, () =>
                {
                    this.AnimateOnEnter(view, entranceTransitionType, () =>
                    {
                        // change region content
                        contentRegionAdapter.OnNavigate(Control, viewOrObject);
                    });
                });

                if (viewOrObjectResult.ResolutionType == ResolutionType.New)
                {
                    AddActiveViewOrObject(sourceType, viewOrObject, context);
                }

                // history
                History.Navigate(navigationEntry);

                if (viewOrObjectResult.ResolutionType == ResolutionType.New || viewOrObjectResult.ResolutionType == ResolutionType.Singleton)
                {
                    // on navigated to
                    if (isView)
                    {
                        OnNavigatedToView(view, parameter);
                        if (context != null)
                            OnNavigatedToContext(context, parameter);
                    }
                }

                this.RaiseNavigated(sourceType, parameter, RegionNavigationType.New);
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
            var currentSourceType = this.History.Current?.SourceType;

            // delay 
            await Task.Delay(1);

            if (await this.ProcessNavigateAsync(sourceType, parameter, entranceTransitionType, exitTransitionType))
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

        private async Task DoSideNavigationAsync(NavigationEntry toGoEntry, RegionNavigationType regionNavigationType,
            Action<object, object> setContentCallback,
            Action onCompleteCallback,
            EntranceTransitionType entranceTransitionType,
            ExitTransitionType exitTransitionType)
        {

            try
            {
                var currentEntry = this.History.Current;
                this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.Back);

                bool currentIsView = false;
                FrameworkElement currentView = null;
                currentIsView = IsView(currentEntry.ViewOrObject);
                if (currentIsView)
                {
                    currentView = currentEntry.ViewOrObject as FrameworkElement;
                }
                object currentContext = currentEntry.Context;

                // Can deactivate current
                if (currentIsView)
                {
                    // throw a navigation failed exception on fail
                    await CheckCanDeactivateChildRegionsAsync(currentEntry.ChildRegions);

                    if (!await CanDeactivateViewAsync(currentView))
                        throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.View, currentView, this);
                }

                if (currentContext != null)
                {
                    if (!await CanDeactivateContextAsync(currentContext))
                        throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, currentContext, this);
                }

                // can activate parent => child regions => sub child regions
                var parameter = toGoEntry.Parameter;
                if (IsView(toGoEntry.ViewOrObject))
                {
                    if (!await CanActivateViewAsync((FrameworkElement)toGoEntry.ViewOrObject, parameter))
                        throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.View, toGoEntry.ViewOrObject, this);

                }
                if (toGoEntry.Context != null)
                {
                    if (!await CanActivateContextAsync(toGoEntry.Context, parameter))
                        throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.Context, toGoEntry.Context, this);

                }
                // child regions
                await CheckCanActivateChildRegionsAsync(toGoEntry.ChildRegions);


                // on navigating from
                this.OnNavigatingFromChildRegions(currentEntry.ChildRegions);
                if (currentIsView)
                {
                    this.OnNavigatingFromView(currentView);
                }

                // on navigating to
                bool isView = IsView(toGoEntry.ViewOrObject);
                FrameworkElement view = null;
                object context = toGoEntry.Context;
                if (isView)
                {
                    view = toGoEntry.ViewOrObject as FrameworkElement;
                    this.OnNavigatingToView(view, parameter);
                }
                if (toGoEntry.Context != null)
                {
                    this.OnNavigatingToContext(context, parameter);
                }
                this.OnNavigatingToChildRegions(toGoEntry.ChildRegions);

                // change content
                var currentRegionContent = contentRegionAdapter.GetContent(Control);
                this.AnimateOnLeave(currentRegionContent, exitTransitionType, () =>
                {
                    this.AnimateOnEnter(view, entranceTransitionType, () =>
                    {
                        setContentCallback(Control, toGoEntry.ViewOrObject);
                    });
                });

                onCompleteCallback();

                // on navigated to
                if (isView)
                {
                    this.OnNavigatedToView(view, parameter);
                }
                if (context != null)
                {
                    this.OnNavigatingToContext(context, parameter);
                }

                this.OnNavigatedToChildRegions(toGoEntry.ChildRegions);
                this.RaiseNavigated(toGoEntry.SourceType, parameter, regionNavigationType);
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
                    (control, view) => contentRegionAdapter.OnGoBack(control, view),
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
                   (control, view) => contentRegionAdapter.OnGoForward(control, view),
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
                  (control, view) => contentRegionAdapter.OnNavigate(control, view),
                  () => History.NavigateToRoot(),
                  entranceTransitionType,
                  exitTransitionType);
            }
        }

    }
}

