using MvvmLib.Commands;
using MvvmLib.History;
using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation source base class.
    /// </summary>
    public class NavigationSource : INotifyCollectionChanged, INotifyPropertyChanged
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

        private readonly ObservableCollection<object> sources;
        /// <summary>
        /// The collection of sources. A source is a View or a ViewModel.
        /// </summary>
        public IReadOnlyCollection<object> Sources
        {
            get { return sources; }
        }

        private object current;
        /// <summary>
        /// The current content (View or a ViewModel).
        /// </summary>
        public object Current
        {
            get { return current; }
        }

        private int currentIndex;
        /// <summary>
        /// The index of the current source selected.
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
        }

        private readonly NavigationHistory history;
        /// <summary>
        /// Gets the history.
        /// </summary>
        public INavigationHistory History
        {
            get { return history; }
        }

        /// <summary>
        /// Checks if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.history.CanGoBack; }
        }

        /// <summary>
        /// Checks if can go forward. 
        /// </summary>
        public bool CanGoForward
        {
            get { return this.history.CanGoForward; }
        }

        private readonly IRelayCommand navigateCommand;
        /// <summary>
        /// Allows to navigate to the source with the source type provided.
        /// </summary>
        public IRelayCommand NavigateCommand
        {
            get { return navigateCommand; }
        }

        private readonly IRelayCommand goBackCommand;
        /// <summary>
        /// Allows to navigate to the previous source.
        /// </summary>
        public IRelayCommand GoBackCommand
        {
            get { return goBackCommand; }
        }

        private readonly IRelayCommand goForwardCommand;
        /// <summary>
        /// Allows to navigate to the next source.
        /// </summary>
        public IRelayCommand GoForwardCommand
        {
            get { return goForwardCommand; }
        }

        private readonly IRelayCommand navigateToRootCommand;
        /// <summary>
        /// Allows to navigate to the first source.
        /// </summary>
        public IRelayCommand NavigateToRootCommand
        {
            get { return navigateToRootCommand; }
        }

        private readonly IRelayCommand redirectCommand;
        /// <summary>
        /// Allows to redirect to the source with the source type provided.
        /// </summary>
        public IRelayCommand RedirectCommand
        {
            get { return redirectCommand; }
        }

        /// <summary>
        /// Invoked when the <see cref="Sources"/> collection has changed.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler<CanGoBackEventArgs> CanGoBackChanged;

        /// <summary>
        /// Invoked when the can go forward value changed.
        /// </summary>
        public event EventHandler<CanGoForwardEventArgs> CanGoForwardChanged;

        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        public event EventHandler<NavigatingEventArgs> Navigating;

        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        public event EventHandler<NavigatedEventArgs> Navigated;

        /// <summary>
        /// Invoked on navigation failed (cancelled or exception).
        /// </summary>
        public event EventHandler<NavigationFailedEventArgs> NavigationFailed;

        /// <summary>
        /// Invoked on current source changed.
        /// </summary>
        public event EventHandler<CurrentSourceChangedEventArgs> CurrentChanged;

        /// <summary>
        /// Creates the navigation source.
        /// </summary>
        public NavigationSource()
        {
            this.sources = new ObservableCollection<object>();
            this.history = new NavigationHistory();

            this.currentIndex = -1;

            navigateCommand = new RelayCommand<Type>(ExecuteNavigateCommand);
            goBackCommand = new RelayCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand);
            goForwardCommand = new RelayCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand);
            navigateToRootCommand = new RelayCommand(ExecuteNavigateToRootCommand, CanExecuteNavigateToRootCommand);
            redirectCommand = new RelayCommand<Type>(ExecuteRedirectCommand);

            history.CanGoBackChanged += OnCanGoBackChanged; ;
            history.CanGoForwardChanged += OnCanGoForwardChanged;
        }

        #region Commands

        private void ExecuteNavigateCommand(Type sourceType)
        {
            this.Navigate(sourceType, null);
        }

        private void ExecuteGoBackCommand()
        {
            this.GoBack();
        }

        private void ExecuteRedirectCommand(Type sourceType)
        {
            this.Redirect(sourceType, null);
        }

        private bool CanExecuteGoBackCommand()
        {
            return this.CanGoBack;
        }

        private void ExecuteGoForwardCommand()
        {
            this.GoForward();
        }

        private bool CanExecuteGoForwardCommand()
        {
            return this.CanGoForward;
        }

        private void ExecuteNavigateToRootCommand()
        {
            this.NavigateToRoot();
        }

        private bool CanExecuteNavigateToRootCommand()
        {
            return this.sources.Count > 0;
        }

        #endregion // Commands

        #region  Events

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            this.CollectionChanged?.Invoke(this, notifyCollectionChangedEventArgs);
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCanGoBackChanged(object sender, CanGoBackEventArgs e)
        {
            NavigateToRootCommand.RaiseCanExecuteChanged();
            GoBackCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoBack));

            this.CanGoBackChanged?.Invoke(this, e);
        }

        private void OnCanGoForwardChanged(object sender, CanGoForwardEventArgs e)
        {
            GoForwardCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoForward));

            this.CanGoForwardChanged?.Invoke(this, e);
        }

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, new CurrentSourceChangedEventArgs(this.currentIndex, this.current));
        }

        private void OnNavigating(NavigationEntry entry, NavigationType navigationType)
        {
            Navigating?.Invoke(this, new NavigatingEventArgs(entry.SourceType, entry.Parameter, navigationType));
        }

        private void OnNavigated(Type sourceType, object parameter, NavigationType navigationType)
        {
            Navigated?.Invoke(this, new NavigatedEventArgs(sourceType, parameter, navigationType));
        }

        private void OnNavigationFailed(NavigationFailedException exception)
        {
            NavigationFailed?.Invoke(this, new NavigationFailedEventArgs(exception));
        }

        private void OnNavigationCompleted(bool success, Type sourceType, object parameter, NavigationType navigationType)
        {
            if (success)
                this.OnNavigated(sourceType, parameter, navigationType);
            else
                this.OnNavigationFailed(new NavigationFailedException(this.current, this));
        }

        #endregion // Events

        private void InsertSource(int index, object source)
        {
            this.sources.Insert(index, source);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, source, index);
        }

        private void RemoveSourceAt(int index)
        {
            var source = sources[index];
            this.sources.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, source, index);
        }

        private void ClearSources(int startIndex)
        {
            if (this.sources.Count - 1 > startIndex)
            {
                // remove from current index to count
                int removeIndex = startIndex + 1;
                while (sources.Count > removeIndex)
                    RemoveSourceAt(removeIndex);
            }
        }

        private void ClearSourcesFast()
        {
            if (this.sources.Count > 0)
            {
                this.sources.Clear();
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// Sets the <see cref="Current"/> value with the new value. Can be override to apply other behaviors.
        /// </summary>
        /// <param name="index">The new index</param>
        /// <param name="source">The source</param>
        protected virtual void SetCurrent(int index, object source)
        {
            this.currentIndex = index;
            this.current = source;
            NavigateToRootCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(CurrentIndex));
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged();
        }

        private void SyncSourcesAndHistoryOnNavigate(Type sourceType, object source, bool isSelectable, NavigationContext navigationContext)
        {
            if (isSelectable)
            {
                int index = sources.IndexOf(source);
                this.SetCurrent(index, source);
                history.MoveTo(index, navigationContext.Parameter);
            }
            else
            {
                // insert source at current index + 1 and clear sources after this index
                var newIndex = this.currentIndex + 1;
                this.InsertSource(newIndex, source);
                this.SetCurrent(newIndex, source);
                ClearSources(newIndex);

                // history: insert entry at current index + 1 and clear entries after this index ("forward stack")
                history.Navigate(new NavigationEntry(sourceType, this.current, navigationContext.Parameter));
            }
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (this.history.Current != null)
                this.OnNavigating(this.history.Current, NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.Navigate(current, sources, navigationContext,
                (source, isSelectable) => SyncSourcesAndHistoryOnNavigate(sourceType, source, isSelectable, navigationContext),
                success => OnNavigationCompleted(success, sourceType, parameter, NavigationType.New));
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void Navigate(Type sourceType)
        {
            this.Navigate(sourceType, null);
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(string sourceName, object parameter)
        {
            if (SourceResolver.TypesForNavigation.TryGetValue(sourceName, out Type sourceType))
                this.Navigate(sourceType, parameter);
            else
                throw new ArgumentException($"No type found for the source name '{sourceName}'. Use 'SourceResolver.RegisterTypeForNavigation' to register the types for names");
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <returns>True on navigation success</returns>
        public void Navigate(string sourceName)
        {
            this.Navigate(sourceName, null);
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Redirect(Type sourceType, object parameter)
        {
            // remove current
            var index = this.history.CurrentIndex;
            if (index >= 0)
            {
                this.history.entries.RemoveAt(index);
                this.history.SetCurrent(index - 1);
                this.sources.RemoveAt(index);
                this.currentIndex = index - 1;
            }
            this.Navigate(sourceType, parameter);
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void Redirect(Type sourceType)
        {
            this.Redirect(sourceType, null);
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <param name="parameter">The parameter</param>
        public void Redirect(string sourceName, object parameter)
        {
            if (SourceResolver.TypesForNavigation.TryGetValue(sourceName, out Type sourceType))
                this.Redirect(sourceType, parameter);
            else
                throw new ArgumentException($"No type found for the source name '{sourceName}'. Use 'SourceResolver.RegisterTypeForNavigation' to register the types for names");
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        public void Redirect(string sourceName)
        {
            this.Redirect(sourceName, null);
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards. Useful for navigation cancellation and not recheck guards.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void NavigateFast(Type sourceType, object parameter)
        {
            if (this.history.Current != null)
                this.OnNavigating(this.history.Current, NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.NavigateFast(current, sources, navigationContext,
                (source, isSelectable) => SyncSourcesAndHistoryOnNavigate(sourceType, source, isSelectable, navigationContext));

            this.OnNavigated(sourceType, parameter, NavigationType.New);
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards. Useful for navigation cancellation and not recheck guards.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void NavigateFast(Type sourceType)
        {
            this.NavigateFast(sourceType, null);
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards. Useful for navigation cancellation and not recheck guards.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        /// <param name="parameter">The parameter</param>
        public void NavigateFast(string sourceName, object parameter)
        {
            if (SourceResolver.TypesForNavigation.TryGetValue(sourceName, out Type sourceType))
                this.NavigateFast(sourceType, parameter);
            else
                throw new ArgumentException($"No type found for the source name '{sourceName}'. Use 'SourceResolver.RegisterTypeForNavigation' to register the types for names");
        }

        /// <summary>
        /// Processes navigation for <see cref="NavigationSource"/> without guards. Useful for navigation cancellation and not recheck guards.
        /// </summary>
        /// <param name="sourceName">The source name</param>
        public void NavigateFast(string sourceName)
        {
            this.NavigateFast(sourceName, null);
        }

        /// <summary>
        /// Navigates to the previous source.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.OnNavigating(this.history.Current, NavigationType.Back);

                var newIndex = this.CurrentIndex - 1;
                var sourceType = history.Previous.SourceType;
                var source = history.Previous.Source;
                var parameter = history.Previous.Parameter;

                var navigationContext = new NavigationContext(sourceType, parameter);
                NavigationHelper.Replace(current, source, navigationContext, () =>
                {
                    SetCurrent(newIndex, source);
                    history.GoBack(navigationContext.Parameter);
                },
                success => OnNavigationCompleted(success, sourceType, parameter, NavigationType.Back));
            }
        }

        /// <summary>
        /// Navigates to the next source.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward)
            {
                this.OnNavigating(this.history.Current, NavigationType.Forward);

                var newIndex = this.CurrentIndex + 1;
                var sourceType = history.Next.SourceType;
                var source = history.Next.Source;
                var parameter = history.Next.Parameter;

                var navigationContext = new NavigationContext(sourceType, parameter);
                NavigationHelper.Replace(current, source, navigationContext, () =>
                {
                    SetCurrent(newIndex, source);
                    history.GoForward(navigationContext.Parameter);
                },
                success => OnNavigationCompleted(success, sourceType, parameter, NavigationType.Forward));
            }
        }

        /// <summary>
        /// Navigates to the first source.
        /// </summary>
        public void NavigateToRoot()
        {
            if (this.sources.Count > 0)
            {
                this.OnNavigating(this.history.Current, NavigationType.Root);

                var newIndex = 0;
                var sourceType = history.Root.SourceType;
                var source = history.Root.Source;
                var parameter = history.Root.Parameter;

                var navigationContext = new NavigationContext(sourceType, parameter);
                NavigationHelper.Replace(current, source, navigationContext, () =>
                {
                    ClearSources(newIndex);
                    SetCurrent(newIndex, source);
                    history.NavigateToRoot(navigationContext.Parameter);
                },
                success => OnNavigationCompleted(success, sourceType, parameter, NavigationType.Root));
            }
        }

        /// <summary>
        /// Navigates to the source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            if (index < 0 || index > this.sources.Count - 1)
                throw new IndexOutOfRangeException();

            int oldIndex = this.currentIndex;

            if (oldIndex == index)
                return; // do not change

            var entry = this.history.entries[index];
            var sourceType = entry.SourceType;
            var source = entry.Source;
            var parameter = entry.Parameter;
            var navigationType = oldIndex > index ? NavigationType.Back : NavigationType.Forward;

            this.OnNavigating(this.history.Current, navigationType);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.Replace(current, source, navigationContext, () =>
            {
                SetCurrent(index, source);
                history.MoveTo(entry, navigationContext.Parameter);
            },
            success => OnNavigationCompleted(success, sourceType, parameter, navigationType));
        }

        /// <summary>
        /// Navigates to the specified source.
        /// </summary>
        /// <param name="source">The source</param>
        public void MoveTo(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var newIndex = this.sources.IndexOf(source);
            if (newIndex == -1)
                throw new ArgumentException("Unable to find the source provided");

            this.MoveTo(newIndex);
        }

        /// <summary>
        /// Clears sources and history.
        /// </summary>
        public void Clear()
        {
            this.history.Clear();
            this.SetCurrent(-1, null);
            this.ClearSourcesFast();
        }

        /// <summary>
        /// Synchronizes the history and sources with the history provided.
        /// </summary>
        /// <param name="history">The navigation history</param>
        public void Sync(INavigationHistory history)
        {
            this.history.Clear();
            this.sources.Clear();

            var currentIndex = history.CurrentIndex;
            object currentSource = null;
            int index = 0;
            foreach (var entry in history.Entries)
            {
                var source = NavigationHelper.EnsureNewView(entry.Source);
                this.history.entries.Add(new NavigationEntry(entry.SourceType, source, entry.Parameter));
                this.sources.Add(source);

                if (index == currentIndex)
                    currentSource = source;
                index++;
            }

            if (currentIndex != -1)
            {
                this.history.SetCurrent(currentIndex);
                this.SetCurrent(currentIndex, currentSource);
            }
        }
    }
}
