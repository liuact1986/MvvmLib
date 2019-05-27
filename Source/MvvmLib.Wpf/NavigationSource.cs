using MvvmLib.Logger;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The NavigationSource class.
    /// </summary>
    public class NavigationSource : INotifyPropertyChanged
    {
        private readonly ILogger DefaultLogger = new DebugLogger();

        private ILogger logger;
        /// <summary>
        /// The logger used by the library.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        /// <summary>
        /// The selectables.
        /// </summary>
        protected readonly Dictionary<Type, List<SelectableRegistration>> selectables;

        /// <summary>
        /// The name / key.
        /// </summary>
        protected string name;
        /// <summary>
        /// The name / key.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        protected readonly NavigationHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public NavigationHistory History
        {
            get { return history; }
        }

        /// <summary>
        /// Chekcs if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.history.CanGoBack; }
        }

        /// <summary>
        /// Chekcs if can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this.history.CanGoForward; }
        }

        private object current;
        /// <summary>
        /// The current source.
        /// </summary>
        public object Current
        {
            get { return current; }
        }

        /// <summary>
        /// Allows to navigate with a source type parameter.
        /// </summary>
        protected IRelayCommand navigateCommand;
        /// <summary>
        /// Allows to navigate with a source type parameter.
        /// </summary>
        public IRelayCommand NavigateCommand
        {
            get { return navigateCommand; }
        }

        /// <summary>
        /// Allows to go back.
        /// </summary>
        protected IRelayCommand goBackCommand;
        /// <summary>
        /// Allows to go back.
        /// </summary>
        public IRelayCommand GoBackCommand
        {
            get { return goBackCommand; }
        }

        /// <summary>
        /// Allows to go forward.
        /// </summary>
        protected IRelayCommand goForwardCommand;
        /// <summary>
        /// Allows to go forward.
        /// </summary>
        public IRelayCommand GoForwardCommand
        {
            get { return goForwardCommand; }
        }

        /// <summary>
        /// Allows to navigate to root.
        /// </summary>
        protected IRelayCommand navigateToRootCommand;
        /// <summary>
        /// Allows to navigate to root.
        /// </summary>
        public IRelayCommand NavigateToRootCommand
        {
            get { return navigateToRootCommand; }
        }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        protected readonly List<EventHandler> canGoBackChanged;
        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged
        {
            add { if (!canGoBackChanged.Contains(value)) canGoBackChanged.Add(value); }
            remove { if (canGoBackChanged.Contains(value)) canGoBackChanged.Remove(value); }
        }

        /// <summary>
        /// Invoked when the can go forward value changed.
        /// </summary>
        protected readonly List<EventHandler> canGoForwardChanged;
        /// <summary>
        /// Invoked when the can go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged
        {
            add { if (!canGoForwardChanged.Contains(value)) canGoForwardChanged.Add(value); }
            remove { if (canGoForwardChanged.Contains(value)) canGoForwardChanged.Remove(value); }
        }

        /// <summary>
        /// Navigating event handlers list.
        /// </summary>
        protected readonly List<EventHandler<NavigationEventArgs>> navigating;
        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigating
        {
            add { if (!navigating.Contains(value)) navigating.Add(value); }
            remove { if (navigating.Contains(value)) navigating.Remove(value); }
        }

        /// <summary>
        /// Navigated event handlers list.
        /// </summary>
        protected readonly List<EventHandler<NavigationEventArgs>> navigated;
        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated
        {
            add { if (!navigated.Contains(value)) navigated.Add(value); }
            remove { if (navigated.Contains(value)) navigated.Remove(value); }
        }

        /// <summary>
        /// NavigationFailed event handlers list.
        /// </summary>
        protected readonly List<EventHandler<NavigationFailedEventArgs>> navigationFailed;

        /// <summary>
        /// Invoked on navigation cancelled or on exception.
        /// </summary>
        public event EventHandler<NavigationFailedEventArgs> NavigationFailed
        {
            add { if (!navigationFailed.Contains(value)) navigationFailed.Add(value); }
            remove { if (navigationFailed.Contains(value)) navigationFailed.Remove(value); }
        }

        /// <summary>
        /// Invoked on current changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates the navigation source.
        /// </summary>
        /// <param name="name">The name / key</param>
        public NavigationSource(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            this.name = name;

            this.selectables = new Dictionary<Type, List<SelectableRegistration>>();
            this.history = new NavigationHistory();

            navigating = new List<EventHandler<NavigationEventArgs>>();
            navigated = new List<EventHandler<NavigationEventArgs>>();
            navigationFailed = new List<EventHandler<NavigationFailedEventArgs>>();

            navigateCommand = new RelayCommand<Type>(async (sourceType) => await ProcessNavigateAsync(sourceType, null));
            goBackCommand = new RelayCommand(async () => await GoBackAsync(), () => CanGoBack);
            goForwardCommand = new RelayCommand(async () => await GoForwardAsync(), () => CanGoForward);
            navigateToRootCommand = new RelayCommand(async () => await NavigateToRootAsync(), () => CanGoBack);

            canGoBackChanged = new List<EventHandler>();
            canGoForwardChanged = new List<EventHandler>();

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
        }

        private void OnCanGoBackChanged(object sender, EventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoBack));

            foreach (var handler in this.canGoBackChanged)
                handler(this, EventArgs.Empty);
        }

        private void OnCanGoForwardChanged(object sender, EventArgs e)
        {
            GoForwardCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoForward));

            foreach (var handler in this.canGoForwardChanged)
                handler(this, EventArgs.Empty);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Selectables

        /// <summary>
        /// Tries to register view or context that implements <see cref="ISelectable"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="source">The source</param>
        /// <param name="context">The context</param>
        protected void TryAddSelectable(Type sourceType, object source, object context)
        {
            if (context is ISelectable)
            {
                if (!selectables.ContainsKey(sourceType))
                    selectables[sourceType] = new List<SelectableRegistration>();

                selectables[sourceType].Add(new SelectableRegistration(true, sourceType, source, context));
            }
            else if (source is ISelectable)
            {
                if (!selectables.ContainsKey(sourceType))
                    selectables[sourceType] = new List<SelectableRegistration>();

                selectables[sourceType].Add(new SelectableRegistration(false, sourceType, source, context));
            }
        }

        /// <summary>
        /// Tries to get the selectable.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The selectable found or null</returns>
        protected SelectableRegistration TryGetSelectable(Type sourceType, object parameter)
        {
            if (selectables.TryGetValue(sourceType, out List<SelectableRegistration> registrations))
            {
                foreach (var registration in registrations)
                {
                    if (registration.IsView)
                    {
                        if (((ISelectable)registration.Context).IsTarget(sourceType, parameter))
                            return registration;
                    }
                    else
                    {
                        if (((ISelectable)registration.Source).IsTarget(sourceType, parameter))
                            return registration;
                    }
                }
            }
            return null;
        }


        /// <summary>
        /// Removes the selectables from source manager.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        /// <returns>True if selectable removed</returns>
        protected bool RemoveSelectable(NavigationEntry entry)
        {
            if (entry.Context is ISelectable || entry.Source is ISelectable)
            {
                var selectable = TryGetSelectable(entry.SourceType, entry.Parameter);
                if (selectable != null)
                {
                    var removed = selectables[entry.SourceType].Remove(selectable);
                    return removed;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the selectables from source manager.
        /// </summary>
        /// <param name="entries">The navigation entries</param>
        protected void RemoveSelectables(IEnumerable<NavigationEntry> entries)
        {
            foreach (var entry in entries)
                RemoveSelectable(entry);
        }

        #endregion // Selectables

        #region Deactivatable management


        /// <summary>
        /// Check Can Deactivate for current entry.
        /// </summary>
        /// <param name="entry">The navigation entry.</param>
        /// <returns></returns>
        protected async Task CheckCanDeactivateOrThrowAsync(NavigationEntry entry)
        {
            var source = entry.Source;
            if (source != null && source is ICanDeactivate)
                if (!await ((ICanDeactivate)source).CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Source, source, this);

            var context = entry.Context;
            if (context != null && context is ICanDeactivate)
                if (!await ((ICanDeactivate)context).CanDeactivateAsync())
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }


        #endregion // Deactivatable management

        #region ICanActivate management


        /// <summary>
        /// Checks can deactivate for source and context that implements <see cref="ICanDeactivate" /> or throws an exception.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="context">The data context</param>
        /// <param name="parameter">the parameter</param>
        /// <returns></returns>
        protected async Task CheckCanActivateOrThrowAsync(object source, object context, object parameter)
        {
            if (source != null && source is ICanActivate)
                if (!await ((ICanActivate)source).CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.ActivationCancelled, NavigationFailedSourceType.Source, source, this);

            if (context != null && context is ICanActivate)
                if (!await ((ICanActivate)context).CanActivateAsync(parameter))
                    throw new NavigationFailedException(NavigationFailedExceptionType.DeactivationCancelled, NavigationFailedSourceType.Context, context, this);
        }


        /// <summary>
        /// Checks can deactivate for source and context that implements <see cref="ICanDeactivate" /> or throws an exception.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        /// <returns></returns>
        protected async Task CheckCanActivateOrThrowAsync(NavigationEntry entry)
        {
            await CheckCanActivateOrThrowAsync(entry.Source, entry.Context, entry.Parameter);
        }

        #endregion // ICanActivate management

        #region INavigatable management

        /// <summary>
        /// Invokes OnNavigatingFrom for source and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatingFrom(NavigationEntry entry)
        {
            var source = entry.Source;
            if (source != null && source is INavigatable)
                ((INavigatable)source).OnNavigatingFrom();

            var context = entry.Context;
            if (context != null && context is INavigatable contextAsINavigatable)
                ((INavigatable)context).OnNavigatingFrom();
        }

        /// <summary>
        /// Invokes OnNavigatingTo for source and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="context">The context</param>
        /// <param name="parameter">The parameter</param>
        protected void OnNavigatingTo(object source, object context, object parameter)
        {
            if (source != null && source is INavigatable)
                ((INavigatable)source).OnNavigatingTo(parameter);

            if (context != null && context is INavigatable)
                ((INavigatable)context).OnNavigatingTo(parameter);
        }


        /// <summary>
        /// Invokes OnNavigatingTo for source and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatingTo(NavigationEntry entry)
        {
            OnNavigatingTo(entry.Source, entry.Context, entry.Parameter);
        }

        /// <summary>
        /// Invokes OnNavigatedTo for source and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="context">The data context</param>
        /// <param name="parameter">The parameter</param>
        protected void OnNavigatedTo(object source, object context, object parameter)
        {
            if (source != null && source is INavigatable)
                ((INavigatable)source).OnNavigatedTo(parameter);

            if (context != null && context is INavigatable)
                ((INavigatable)context).OnNavigatedTo(parameter);
        }


        /// <summary>
        /// Invokes OnNavigatedTo for source and context that implements <see cref="INavigatable" />.
        /// </summary>
        /// <param name="entry">The navigation entry</param>
        protected void OnNavigatedTo(NavigationEntry entry)
        {
            OnNavigatedTo(entry.Source, entry.Context, entry.Parameter);
        }

        #endregion INavigatable management


        /// <summary>
        /// Creates an new instance of the type.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The new instance</returns>
        public object CreateInstance(Type sourceType)
        {
            var source = ViewResolver.CreateInstance(sourceType);
            return source;
        }

        /// <summary>
        /// Gets or set the context.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="source">The source</param>
        /// <returns>The context or null</returns>
        protected object GetOrSetContext(Type sourceType, object source)
        {
            object context = null;
            var view = source as FrameworkElement;
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

        /// <summary>
        /// Resolves the view model model for the view type.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <returns>The view model</returns>
        protected object ResolveContextWithViewModelLocator(Type viewType)
        {
            var viewModelType = ViewModelLocationProvider.ResolveViewModelType(viewType); // singleton or new instance
            if (viewModelType != null)
            {
                var context = ViewModelLocationProvider.CreateViewModelInstance(viewModelType);
                return context;
            }
            return null;
        }

        /// <summary>
        /// Notifies <see cref="Navigating"/> subscribers.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter type</param>
        /// <param name="navigationType">The navigation type</param>
        protected void OnNavigating(Type sourceType, object parameter, NavigationType navigationType)
        {
            var context = new NavigationEventArgs(sourceType, parameter, navigationType);
            foreach (var handler in this.navigating)
                handler(this, context);
        }

        /// <summary>
        /// Notifies <see cref="Navigated"/> subscribers.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter type</param>
        /// <param name="navigationType">The navigation type</param>
        protected void OnNavigated(Type sourceType, object parameter, NavigationType navigationType)
        {
            var context = new NavigationEventArgs(sourceType, parameter, navigationType);
            foreach (var handler in this.navigated)
                handler(this, context);
        }

        /// <summary>
        ///  Notifies <see cref="NavigationFailed"/> subscribers.
        /// </summary>
        /// <param name="exception">The exception</param>
        protected void OnNavigationFailed(NavigationFailedException exception)
        {
            var context = new NavigationFailedEventArgs(exception);
            foreach (var handler in this.navigationFailed)
                handler(this, context);
        }

        /// <summary>
        /// Sets the <see cref="Current"/> value with the new value. Can be override to apply other behaviors.
        /// </summary>
        /// <param name="source">The source</param>
        protected virtual void ChangeContent(object source)
        {
            current = source;
            OnPropertyChanged(nameof(Current));
        }

        /// <summary>
        /// Clears the content.
        /// </summary>
        public void ClearContent()
        {
            ChangeContent(null);
        }

        /// <summary>
        /// Navigates to the source and notify the view model.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True on navigation  success</returns>
        protected async Task<bool> ProcessNavigateAsync(Type sourceType, object parameter)
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
                    this.OnNavigating(currentEntry.SourceType, currentEntry.Parameter, NavigationType.New);

                    // Can Deactivate
                    await CheckCanDeactivateOrThrowAsync(currentEntry);
                }

                bool isNew = true;
                object source = null;
                object context = null;
                var selectableRegistration = TryGetSelectable(sourceType, parameter);
                if (selectableRegistration != null)
                {
                    if (selectableRegistration.IsView)
                    {
                        source = selectableRegistration.Source;
                        context = selectableRegistration.Context;
                    }
                    else
                    {
                        // change content to existing vieworobject, vieworobject is viewmodel (context is null)
                        source = selectableRegistration.Source;
                    }
                    isNew = false;
                }
                else
                {
                    source = CreateInstance(sourceType);
                    context = GetOrSetContext(sourceType, source);
                }

                // Can Activate
                await CheckCanActivateOrThrowAsync(source, context, parameter);

                // on navigating from
                if (currentEntry != null)
                    OnNavigatingFrom(currentEntry);

                // on navigating to
                if (isNew)
                    OnNavigatingTo(source, context, parameter);

                // change content
                ChangeContent(source);

                // on navigated to
                if (isNew)
                    OnNavigatedTo(source, context, parameter);

                // clear selectables for forward stack
                RemoveSelectables(history.ForwardStack);
                // history
                history.Navigate(new NavigationEntry(sourceType, source, parameter, context));

                if (isNew)
                    TryAddSelectable(sourceType, source, context);

                // navigated event
                this.OnNavigated(sourceType, parameter, NavigationType.New);
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

        /// <summary>
        /// Process side Navigation (go back, go forward, navigate to root).
        /// </summary>
        /// <param name="entryToGo">The entry</param>
        /// <param name="navigationType">The navigation type</param>
        /// <param name="updateHistoryCallback">The callback used to update the history</param>
        /// <returns>True on navigation  success</returns>
        protected async Task<bool> DoSideNavigationAsync(NavigationEntry entryToGo, NavigationType navigationType, Action updateHistoryCallback)
        {
            var navigationSuccess = true;

            try
            {
                var currentEntry = this.history.Current;
                // navigating event
                this.OnNavigating(currentEntry.SourceType, currentEntry.Parameter, NavigationType.New);

                // Can Deactivate
                await CheckCanDeactivateOrThrowAsync(currentEntry);

                // Can Activate
                await CheckCanActivateOrThrowAsync(entryToGo);

                // on navigating from
                this.OnNavigatingFrom(currentEntry);

                // on navigating
                OnNavigatingTo(entryToGo);

                // change content
                var source = entryToGo.Source;
                ChangeContent(source);

                // on navigated to
                OnNavigatedTo(entryToGo);

                // history
                updateHistoryCallback();

                // navigated event
                this.OnNavigated(entryToGo.SourceType, entryToGo.Parameter, navigationType);
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
                return await this.DoSideNavigationAsync(history.Previous, NavigationType.Back, () => history.GoBack());

            return false;
        }

        /// <summary>
        /// Navigates to next entry.
        /// </summary>
        /// <returns>True on navigation  success</returns>
        public async Task<bool> GoForwardAsync()
        {
            if (this.CanGoForward)
                return await this.DoSideNavigationAsync(history.Next, NavigationType.Forward, () => history.GoForward());

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
                return await this.DoSideNavigationAsync(history.Root, NavigationType.Root,
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
                        RemoveSelectable(history.Current);
                        // history
                        history.NavigateToRoot();
                    });
            }

            return false;
        }
    }

    /// <summary>
    /// NavigationSource for <see cref="ContentControl"/>.
    /// </summary>
    public class ContentControlNavigationSource : NavigationSource
    {
        private ContentControl control;
        /// <summary>
        /// The ContentControl.
        /// </summary>
        public ContentControl Control
        {
            get { return control; }
        }

        /// <summary>
        /// Creates the ContentControl NavigationSource.
        /// </summary>
        /// <param name="name">The name / key</param>
        /// <param name="control">The ContentControl</param>
        public ContentControlNavigationSource(string name, ContentControl control)
            : base(name)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            this.control = control;
            this.control.Unloaded += OnUnloaded;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.control.Unloaded -= OnUnloaded;
            if (NavigationManager.RemoveNavigationSource(name))
                Logger.Log($"ControlControlNavigationSource \"{name}\" unregistered on control unloaded", Category.Info, Priority.Low);
            else
                Logger.Log($"Unable to unregister ControlControlNavigationSource \"{name}\" on control unloaded", Category.Debug, Priority.High);
        }

        /// <summary>
        /// Sets the content of the ContentControl.
        /// </summary>
        /// <param name="source">The new content</param>
        protected override void ChangeContent(object source)
        {
            control.Content = source;
            base.ChangeContent(source);
        }
    }
}
