using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Content Region class.
    /// </summary>
    public sealed class ContentRegion : RegionBase
    {
        private readonly ContentRegionHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public ContentRegionHistory History
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

        private readonly List<EventHandler> canGoBackChanged;
        /// <summary>
        /// Invoked when the region can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged
        {
            add { if (!canGoBackChanged.Contains(value)) canGoBackChanged.Add(value); }
            remove { if (canGoBackChanged.Contains(value)) canGoBackChanged.Remove(value); }
        }

        private readonly List<EventHandler> canGoForwardChanged;
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
        /// <param name="regionName">The region name</param>
        /// <param name="control">The control</param>
        /// <param name="regionsRegistry">The regions registry</param>
        public ContentRegion(string regionName, FrameworkElement control, RegionsRegistry regionsRegistry)
            : base(regionName, control, regionsRegistry)
        {
            if (control is ContentControl contentControl)
            {
                if (contentControl.Content != null || (BindingOperations.GetBinding(contentControl, ContentControl.ContentProperty) != null))
                    throw new InvalidOperationException("ContentControl is not empty or binded");
            }
            else
                throw new NotSupportedException($"Only \"ContentControl\" is supported for ContentRegion. Type of \"{control.GetType().Name}\"");

            this.history = new ContentRegionHistory();

            canGoBackChanged = new List<EventHandler>();
            canGoForwardChanged = new List<EventHandler>();

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
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

        private void ChangeContent(object content)
        {
            ((ContentControl)control).Content = content;
        }

        /// <summary>
        /// Clear content with no animation.
        /// </summary>
        internal void ClearContent()
        {
            ChangeContent(null);
        }

        private async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            bool navigationSuccess = true;

            try
            {
                var currentEntry = this.history.Current;

                if (currentEntry != null)
                {
                    // navigating event
                    this.OnNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                    // Can Deactivate
                    await CheckCanDeactivateOrThrowAsync(currentEntry);
                }

                bool isNew = true;
                object viewOrObject = null;
                object context = null;
                var selectableRegistration = TryGetSelectable(sourceType, parameter);
                if (selectableRegistration != null)
                {
                    if (selectableRegistration.IsView)
                    {
                        viewOrObject = selectableRegistration.ViewOrObject;
                        context = selectableRegistration.Context;
                    }
                    else
                    {
                        // change content to existing vieworobject, vieworobject is viewmodel (context is null)
                        viewOrObject = selectableRegistration.ViewOrObject;
                    }
                    isNew = false;
                }
                else
                {
                    viewOrObject = CreateInstance(sourceType);
                    context = GetOrSetContext(sourceType, viewOrObject);
                }

                // Can Activate
                await CheckCanActivateOrThrowAsync(viewOrObject, context, parameter);

                // loaded
                var view = viewOrObject as FrameworkElement;
                var navigationEntry = new NavigationEntry(sourceType, viewOrObject, parameter, context);
                if (isNew && view != null)
                {
                    HandleViewLoaded(view, (s, e) =>
                    {
                        var childRegions = FindChildRegions((DependencyObject)s);
                        navigationEntry.childRegions = childRegions;

                        this.NotifyLoadedListeners(viewOrObject, context, parameter);
                    });
                }

                if (context != null)
                    this.NotifyRegionKnowledge(this, context); // ?

                // on navigating from
                if (currentEntry != null)
                    OnNavigatingFrom(currentEntry);

                // on navigating to
                if (isNew)
                    OnNavigatingTo(viewOrObject, context, parameter);

                // change content
                ChangeContent(viewOrObject);

                if (isNew && view == null)
                {
                    await Task.Delay(1);

                    var childRegions = FindChildRegions(control);
                    navigationEntry.childRegions = childRegions;

                    this.NotifyLoadedListeners(viewOrObject, context, parameter);
                }

                // on navigated to
                if (isNew)
                    OnNavigatedTo(viewOrObject, context, parameter);

                // clear selectables for forward stack
                RemoveSelectables(history.ForwardStack);
                // history
                history.Navigate(navigationEntry);

                if (isNew)
                    TryAddSelectable(sourceType, viewOrObject, context);

                // navigated event
                this.OnNavigated(sourceType, parameter, RegionNavigationType.New);
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
        /// Navigates to the source and notify the view model.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(Type sourceType)
        {
            return await this.ProcessNavigateAsync(sourceType, null);
        }

        /// <summary>
        /// Navigates to the source and notify the view model.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> NavigateAsync(Type sourceType, object parameter)
        {
            return await this.ProcessNavigateAsync(sourceType, parameter);
        }

        /// <summary>
        /// Redirects to the view and remove the previous entry from history.
        /// </summary>
        /// <param name="sourceType">The type of the view or view model to redirect</param>
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
                        RemoveSelectable(entry);
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
        /// <param name="sourceType">The type of the view or view model to redirect</param>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> RedirectAsync(Type sourceType)
        {
            return await RedirectAsync(sourceType, null);
        }

        private async Task<bool> DoSideNavigationAsync(NavigationEntry entryToGo, RegionNavigationType regionNavigationType, Action updateHistoryCallback)
        {
            var navigationSuccess = true;

            try
            {
                var currentEntry = this.history.Current;
                // navigating event
                this.OnNavigating(currentEntry.SourceType, currentEntry.Parameter, RegionNavigationType.New);

                // Can Deactivate
                await CheckCanDeactivateOrThrowAsync(currentEntry);

                // Can Activate
                await CheckCanActivateOrThrowAsync(entryToGo);

                // on navigating from
                this.OnNavigatingFrom(currentEntry);

                // on navigating
                OnNavigatingTo(entryToGo);

                // change content
                var viewOrObject = entryToGo.Source;
                ChangeContent(viewOrObject);

                // on navigated to
                OnNavigatedTo(entryToGo);

                // history
                updateHistoryCallback();

                // navigated event
                this.OnNavigated(entryToGo.SourceType, entryToGo.Parameter, regionNavigationType);
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
        /// Navigates to previous entry.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoBackAsync()
        {
            if (this.CanGoBack)
                return await this.DoSideNavigationAsync(history.Previous, RegionNavigationType.Back, () => history.GoBack());

            return false;
        }

        /// <summary>
        /// Navigates to next entry.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoForwardAsync()
        {
            if (this.CanGoForward)
                return await this.DoSideNavigationAsync(history.Next, RegionNavigationType.Forward, () => history.GoForward());

            return false;
        }

        /// <summary>
        /// Navigates to the root entry.
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
                        RemoveSelectable(CurrentEntry);
                        // history
                        history.NavigateToRoot();
                    });
            }

            return false;
        }
    }
}

