using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The Content Region class.
    /// </summary>
    public sealed class ContentRegion : RegionBase
    {
        private ViewOrObjectManager ViewOrObjectManager;
        private IContentRegionAnimation defaultRegionNavigationAnimation;

        /// <summary>
        /// The default animation for the region.
        /// </summary>
        public IContentRegionAnimation DefaultRegionAnimation
        {
            get { return defaultRegionNavigationAnimation; }
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
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ContentRegion(INavigationHistory history, string regionName, object control)
            : base(regionName, control)
        {
            if (!(control is ContentControl)) { throw new NotSupportedException($"Only\"ContentControls\" are supported for ContentRegion. Type of {control.GetType().Name}"); }

            this.History = history;
            this.ViewOrObjectManager = new ViewOrObjectManager();

            ((FrameworkElement)control).Unloaded += OnControlUnloaded;

            this.defaultRegionNavigationAnimation = new ContentRegionAnimation((ContentControl)control);

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
        }

        private void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            History.CanGoBackChanged -= OnCanGoBackChanged;
            History.CanGoForwardChanged -= OnCanGoForwardChanged;

            ClearRegionAndChildRegions(History.ForwardStack);
            ClearRegionAndChildRegions(History.BackStack);
            if (History.Current != null)
                ClearChildRegions(History.Current);
            if (!RegionManager.UnregisterContentRegion(this))
                this.Logger.Log($"Failed to unregister the content region \"{RegionName}\", control name:\"{ControlName}\"", Category.Exception, Priority.High);
        }

        /// <summary>
        /// Creates an ContentRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        public ContentRegion(string regionName, object control)
            : this(new NavigationHistory(), regionName, control)
        { }

        /// <summary>
        /// Short hand to configure kickly the default animation.
        /// </summary>
        /// <param name="entranceAnimation">The entrance animation</param>
        /// <param name="exitAnimation">The exite animation</param>
        /// <param name="simultaneous">Playing behavior</param>
        public void ConfigureAnimation(IContentAnimation entranceAnimation, IContentAnimation exitAnimation, bool simultaneous = false)
        {
            defaultRegionNavigationAnimation.EntranceAnimation = entranceAnimation;
            defaultRegionNavigationAnimation.ExitAnimation = exitAnimation;
            defaultRegionNavigationAnimation.Simultaneous = simultaneous;
        }

        private void OnCanGoBackChanged(object sender, EventArgs e)
        {
            foreach (var handler in this.canGoBackChanged)
                handler(this, EventArgs.Empty);
        }

        private void OnCanGoForwardChanged(object sender, EventArgs e)
        {
            foreach (var handler in this.canGoForwardChanged)
                handler(this, EventArgs.Empty);
        }

        #region View or object management

        private void TryAddViewOrObject(Type sourceType, object viewOrObject, object context)
        {
            ViewOrObjectManager.TryAddViewOrObject(sourceType, viewOrObject, context);
        }

        private ViewOrObjectInstanceResult GetOrCreateViewOrObjectInstance(Type sourceType, object parameter)
        {
            return ViewOrObjectManager.GetOrCreateViewOrObjectInstance(sourceType, parameter);
        }

        #endregion // View or object management

        private object GetOrSetContext(ViewOrObjectInstanceResult viewOrObjectResult)
        {
            object context = null;
            if (viewOrObjectResult.ResolutionType == ResolutionType.New)
            {
                var view = viewOrObjectResult.Instance as FrameworkElement;
                if (view != null)
                {
                    if (view.DataContext != null)
                        context = view.DataContext;
                    else
                    {
                        context = ResolveContextWithViewModelLocator(viewOrObjectResult.SourceType);
                        if (context != null)
                            view.DataContext = context;
                    }
                }
            }
            else
            {
                var view = viewOrObjectResult.Instance as FrameworkElement;
                if (view != null && view.DataContext != null)
                    context = view.DataContext;
            }
            return context;
        }

        /// <summary>
        /// Clear content with no animation.
        /// </summary>
        internal void ClearContent()
        {
            ((ContentControl)Control).Content = null;
        }

        private void ClearRegionAndChildRegions(IEnumerable<NavigationEntry> entries)
        {
            foreach (var entry in entries)
            {
                // sub child => child => parent
                if (entry.ChildRegions.Count > 0)
                    ClearChildRegions(entry);

                // current
                ViewOrObjectManager.RemoveSelectable(entry.SourceType);
            }
        }

        private async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter, IContentRegionAnimation regionAnimation)
        {
            if (sourceType == null) { throw new ArgumentNullException(nameof(sourceType)); }

            if (regionAnimation.IsAnimating) return false;

            bool navigationSuccess = true;

            try
            {
                var currentEntry = this.History.Current;

                if (currentEntry != null)
                {
                    // navigating event
                    this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                    // Can Deactivate
                    await CheckCanDeactivateAsync(currentEntry);
                }

                // view or object
                var viewOrObjectResult = GetOrCreateViewOrObjectInstance(sourceType, parameter);
                // context
                var context = GetOrSetContext(viewOrObjectResult);

                object viewOrObject = viewOrObjectResult.Instance;

                // Can Activate
                await CheckCanActivateAsync(viewOrObject, context, parameter);

                // loaded
                var view = viewOrObject as FrameworkElement;
                var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);
                if (viewOrObjectResult.ResolutionType == ResolutionType.New && view != null)
                {
                    HandleLoaded(view, (s, e) =>
                    {
                        var childRegions = FindChildRegions((DependencyObject)s);
                        navigationEntry.ChildRegions = childRegions;

                        this.NotifyLoadedListeners(view, context, parameter);
                    });
                }

                if (context != null)
                    this.NotifyRegionKnowledge(this, context); // ?

                // on navigating from
                if (currentEntry != null)
                    OnNavigatingFrom(currentEntry);

                // on navigating to
                if (viewOrObjectResult.ResolutionType == ResolutionType.New || viewOrObjectResult.ResolutionType == ResolutionType.Singleton)
                    OnNavigatingTo(viewOrObject, context, parameter);

                // change content
                regionAnimation.Start(viewOrObject);

                // on navigated to
                if (viewOrObjectResult.ResolutionType == ResolutionType.New || viewOrObjectResult.ResolutionType == ResolutionType.Singleton)
                    OnNavigatedTo(viewOrObject, context, parameter);

                // clear region and child regions for forward stack
                ClearRegionAndChildRegions(History.ForwardStack);
                // hsitory
                History.Navigate(navigationEntry);

                if (viewOrObjectResult.ResolutionType == ResolutionType.New)
                    TryAddViewOrObject(sourceType, viewOrObject, context);

                // navigated event
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
                RegionManager.RemoveNonLoadedRegions();

            return navigationSuccess;
        }

        /// <summary>
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourceType)
        {
            await this.ProcessNavigateAsync(sourceType, null, defaultRegionNavigationAnimation);
        }

        /// <summary>
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourceType, object parameter)
        {
            await this.ProcessNavigateAsync(sourceType, parameter, defaultRegionNavigationAnimation);
        }


        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourceType, object parameter)
        {
            var currentSourceType = this.History.Current?.SourceType;

            // delay 
            await Task.Delay(1);

            if (await this.ProcessNavigateAsync(sourceType, parameter, defaultRegionNavigationAnimation))
            {
                if (currentSourceType != null)
                {
                    // remove page from history
                    var entry = this.History.Previous;
                    if (entry != null && entry.SourceType == currentSourceType)
                    {
                        ClearChildRegions(entry);
                        this.History.BackStack.Remove(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourceType)
        {
            await RedirectAsync(sourceType, null);
        }

        private async Task DoSideNavigationAsync(NavigationEntry toGoEntry, RegionNavigationType regionNavigationType,
            Action updateHistoryCallback,
            IContentRegionAnimation regionAnimation)
        {

            if (regionAnimation.IsAnimating) return;

            try
            {
                var currentEntry = this.History.Current;
                // navigating event
                this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                // Can Deactivate
                await CheckCanDeactivateAsync(currentEntry);

                // Can Activate
                await CheckCanActivateAsync(toGoEntry);

                // on navigating from
                this.OnNavigatingFrom(currentEntry);

                // on navigating
                OnNavigatingTo(toGoEntry);

                // change content
                var viewOrObject = toGoEntry.ViewOrObject;
                regionAnimation.Start(viewOrObject);

                // on navigated to
                OnNavigatedTo(toGoEntry);

                // history
                updateHistoryCallback();

                // navigated event
                this.RaiseNavigated(toGoEntry.SourceType, toGoEntry.Parameter, regionNavigationType);
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
        /// <returns></returns>
        public async Task GoBackAsync()
        {
            if (this.CanGoBack)
            {
                await this.DoSideNavigationAsync(History.Previous, RegionNavigationType.Back,
                    () => History.GoBack(),
                    defaultRegionNavigationAnimation);
            }
        }


        /// <summary>
        /// Navigates to next view.
        /// </summary>
        /// <returns></returns>
        public async Task GoForwardAsync()
        {
            if (this.CanGoForward)
            {
                await this.DoSideNavigationAsync(History.Next, RegionNavigationType.Forward,
                   () => History.GoForward(),
                   defaultRegionNavigationAnimation);
            }
        }


        /// <summary>
        /// Navigates to the root view.
        /// </summary>
        /// <returns></returns>
        public async Task NavigateToRootAsync()
        {
            if (this.CanGoBack)
            {
                await this.DoSideNavigationAsync(History.Root, RegionNavigationType.Root,
                  () =>
                  {
                      ClearRegionAndChildRegions(History.BackStack);
                      ClearRegionAndChildRegions(History.ForwardStack);
                      History.NavigateToRoot();
                  },
                  defaultRegionNavigationAnimation);
            }
        }
    }
}

