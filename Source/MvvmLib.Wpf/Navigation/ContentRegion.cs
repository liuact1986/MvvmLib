using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly INavigationHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public INavigationHistory History
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
        /// Chekcs if the content region can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.history.BackStack.Count > 0; }
        }

        /// <summary>
        /// Chekcs if the content region can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this.history.ForwardStack.Count > 0; }
        }

        private readonly List<EventHandler> canGoBackChanged = new List<EventHandler>();
        /// <summary>
        /// Invoked when the region can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged
        {
            add { if (!canGoBackChanged.Contains(value)) canGoBackChanged.Add(value); }
            remove { if (canGoBackChanged.Contains(value)) canGoBackChanged.Remove(value); }
        }

        private readonly List<EventHandler> canGoForwardChanged = new List<EventHandler>();
        /// <summary>
        /// Invoked when the region can go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged
        {
            add { if (!canGoForwardChanged.Contains(value)) canGoForwardChanged.Add(value); }
            remove { if (canGoForwardChanged.Contains(value)) canGoForwardChanged.Remove(value); }
        }

        /// <summary>
        /// Creates an ContentRegion.
        /// </summary>
        /// <param name="viewOrObjectManager">The view or object manager</param>
        /// <param name="history">The history</param>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ContentRegion(ViewOrObjectManager viewOrObjectManager, INavigationHistory history, string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : base(viewOrObjectManager, regionName, control, regionsRegistry)
        {
            if (!(control is ContentControl)) { throw new NotSupportedException($"Only\"ContentControls\" are supported for ContentRegion. Type of {control.GetType().Name}"); }

            this.history = history;

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
        }

        /// <summary>
        /// Creates an ContentRegion.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ContentRegion(string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : this(new ViewOrObjectManager(), new NavigationHistory(), regionName, control, regionsRegistry)
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
            if (history.ForwardStack.Count > 0)
            {
                RemoveSelectables(History.ForwardStack);
                RemoveSingletons(History.ForwardStack);
            }
            if (history.BackStack.Count > 0)
            {
                RemoveSelectables(History.BackStack);
                RemoveSingletons(History.BackStack);
            }
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

        private void ChangeContent(object viewOrObject)
        {
            ((ContentControl)control).Content = viewOrObject;
        }

        #region View or object management

        private void TryAddViewOrObject(Type sourceType, object viewOrObject, object context)
        {
            viewOrObjectManager.TryAddViewOrObject(sourceType, viewOrObject, context);
        }

        private ViewOrObjectInstanceResult GetOrCreateViewOrObjectInstance(Type sourceType, object parameter)
        {
            return viewOrObjectManager.GetOrCreateViewOrObjectInstance(sourceType, parameter);
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

        private async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter)
        {
            if (sourceType == null) { throw new ArgumentNullException(nameof(sourceType)); }

            bool navigationSuccess = true;

            try
            {
                var currentEntry = this.history.Current;

                if (currentEntry != null)
                {
                    // navigating event
                    this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                    // Can Deactivate
                    await CheckCanDeactivateOrThrowAsync(currentEntry);
                }

                // view or object
                var viewOrObjectResult = GetOrCreateViewOrObjectInstance(sourceType, parameter);
                // context
                var context = GetOrSetContext(viewOrObjectResult);

                object viewOrObject = viewOrObjectResult.Instance;

                // Can Activate
                await CheckCanActivateOrThrowAsync(viewOrObject, context, parameter);

                // loaded and unloaded
                var view = viewOrObject as FrameworkElement;
                var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);
                if (viewOrObjectResult.ResolutionType == ResolutionType.New && view != null)
                {
                    HandleViewLoaded(view, (s, e) =>
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
                ChangeContent(viewOrObject);


                // on navigated to
                if (viewOrObjectResult.ResolutionType == ResolutionType.New || viewOrObjectResult.ResolutionType == ResolutionType.Singleton)
                    OnNavigatedTo(viewOrObject, context, parameter);

                // clear selectables for forward stack
                RemoveSelectables(history.ForwardStack);
                // history
                history.Navigate(navigationEntry);

                if (viewOrObjectResult.ResolutionType == ResolutionType.New)
                    TryAddViewOrObject(sourceType, viewOrObject, context);

                // navigated event
                this.RaiseNavigated(sourceType, parameter, RegionNavigationType.New);
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
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(Type sourceType)
        {
            return await this.ProcessNavigateAsync(sourceType, null);
        }

        /// <summary>
        /// Navigates to view and notify viewmodel.
        /// </summary>
        /// <param name="sourceType">The view type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(Type sourceType, object parameter)
        {
            return await this.ProcessNavigateAsync(sourceType, parameter);
        }

        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> RedirectAsync(Type sourceType, object parameter)
        {
            var currentSourceType = this.history.Current?.SourceType;

            // delay 
            await Task.Delay(1);

            if (await this.ProcessNavigateAsync(sourceType, parameter))
            {
                if (currentSourceType != null)
                {
                    // remove page from history
                    var entry = this.history.Previous;
                    if (entry != null && entry.SourceType == currentSourceType)
                    {
                        ClearChildRegions(entry);
                        this.history.BackStack.Remove(entry);
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Redirect to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view to redirect</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> RedirectAsync(Type sourceType)
        {
            return await RedirectAsync(sourceType, null);
        }

        private async Task<bool> DoSideNavigationAsync(NavigationEntry toGoEntry, RegionNavigationType regionNavigationType, Action updateHistoryCallback)
        {
            var navigationSuccess = true;

            try
            {
                var currentEntry = this.history.Current;
                // navigating event
                this.RaiseNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                // Can Deactivate
                await CheckCanDeactivateOrThrowAsync(currentEntry);

                // Can Activate
                await CheckCanActivateOrThrowAsync(toGoEntry);

                // on navigating from
                this.OnNavigatingFrom(currentEntry);

                // on navigating
                OnNavigatingTo(toGoEntry);

                // change content
                var viewOrObject = toGoEntry.ViewOrObject;
                ChangeContent(viewOrObject);

                // on navigated to
                OnNavigatedTo(toGoEntry);

                // history
                updateHistoryCallback();

                // navigated event
                this.RaiseNavigated(toGoEntry.SourceType, toGoEntry.Parameter, regionNavigationType);
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
        /// Navigates to previous view.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoBackAsync()
        {
            if (this.CanGoBack)
                return await this.DoSideNavigationAsync(history.Previous, RegionNavigationType.Back, () => history.GoBack());

            return false;
        }

        /// <summary>
        /// Navigates to next view.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoForwardAsync()
        {
            if (this.CanGoForward)
                return await this.DoSideNavigationAsync(history.Next, RegionNavigationType.Forward, () => history.GoForward());

            return false;
        }

        /// <summary>
        /// Navigates to the root view.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateToRootAsync()
        {
            if (this.CanGoBack)
            {
                return await this.DoSideNavigationAsync(history.Root, RegionNavigationType.Root,
                     () =>
                     {
                         RemoveSelectables(history.ForwardStack);
                         // backstack 1 => end
                         if (history.BackStack.Count > 0)
                         {
                             var backStack = history.BackStack.Skip(1);
                             RemoveSelectables(backStack);
                         }
                         // current
                         RemoveSelectables(CurrentEntry.SourceType);
                         // history
                         history.NavigateToRoot();
                     });
            }

            return false;
        }
    }
}

