using MvvmLib.Commands;
using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation source base class.
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

        private readonly ObservableCollection<object> sources;
        /// <summary>
        /// The collection of sources. A source is a View or a ViewModel.
        /// </summary>
        public IReadOnlyCollection<object> Sources
        {
            get { return sources; }
        }

        private readonly NavigationEntryCollection entries;
        /// <summary>
        /// The entry collection.
        /// </summary>
        public IReadOnlyCollection<NavigationEntry> Entries
        {
            get { return entries; }
        }

        private object current;
        /// <summary>
        /// The current source (View or a ViewModel).
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

        /// <summary>
        /// Chekcs if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.currentIndex > 0; }
        }

        /// <summary>
        /// Chekcs if can go forward.
        /// </summary>
        public bool CanGoForward
        {
            get { return this.currentIndex < this.sources.Count - 1; }
        }

        private bool clearSourcesOnNavigate;
        /// <summary>
        /// Allows to remove "forward stack" on navigate.
        /// </summary>
        public bool ClearSourcesOnNavigate
        {
            get { return clearSourcesOnNavigate; }
            set { clearSourcesOnNavigate = value; }
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

        private readonly IRelayCommand moveToLastCommand;
        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        public IRelayCommand MoveToLastCommand
        {
            get { return moveToLastCommand; }
        }

        private readonly IRelayCommand moveToIndexCommand;
        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        public IRelayCommand MoveToIndexCommand
        {
            get { return moveToIndexCommand; }
        }

        private readonly IRelayCommand moveToCommand;
        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        public IRelayCommand MoveToCommand
        {
            get { return moveToCommand; }
        }

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
            this.entries = new NavigationEntryCollection();
            this.currentIndex = -1;
            this.clearSourcesOnNavigate = true;

            navigateCommand = new RelayCommand<Type>(ExecuteNavigateCommand);
            goBackCommand = new RelayCommand(ExecuteGoBackCommand, CanExecuteGoBackCommand);
            goForwardCommand = new RelayCommand(ExecuteGoForwardCommand, CanExecuteGoForwardCommand);
            navigateToRootCommand = new RelayCommand(ExecuteNavigateToRootCommand, CanExecuteNavigateToRootCommand);
            redirectCommand = new RelayCommand<Type>(ExecuteRedirectCommand);
            moveToLastCommand = new RelayCommand(ExecuteMoveToLastCommand, CanExecuteMoveToLastCommand);
            moveToIndexCommand = new RelayCommand<int>(ExecuteMoveToIndexCommand);
            moveToCommand = new RelayCommand<object>(ExecuteMoveToCommand);
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

        private void ExecuteMoveToLastCommand()
        {
            this.MoveToLast();
        }

        private bool CanExecuteMoveToLastCommand()
        {
            return this.sources.Count > 0 && currentIndex < this.sources.Count;
        }

        private void ExecuteMoveToIndexCommand(int index)
        {
            this.MoveTo(index);
        }

        private void ExecuteMoveToCommand(object source)
        {
            this.MoveTo(source);
        }

        #endregion // Commands

        #region  Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCanGobackChanged(bool canGoBack)
        {
            navigateToRootCommand.RaiseCanExecuteChanged();
            goBackCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoBack));

            this.CanGoBackChanged?.Invoke(this, new CanGoBackEventArgs(canGoBack));
        }

        private void OnCanGoForwardChanged(bool canGoForward)
        {
            moveToLastCommand.RaiseCanExecuteChanged();
            goForwardCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanGoForward));

            this.CanGoForwardChanged?.Invoke(this, new CanGoForwardEventArgs(canGoForward));
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

        #region Sources management

        private void SetCurrentIndex(int index)
        {
            currentIndex = index;
            OnPropertyChanged(nameof(CurrentIndex));
        }

        private void UpdateEntryParameter(int index, object parameter)
        {
            if (index >= 0)
                this.entries[index].Parameter = parameter;
        }

        private void CheckCanGoBackAndCanGoForward(bool oldCanGoBack, bool oldCanGoForward)
        {
            if (CanGoBack != oldCanGoBack)
                OnCanGobackChanged(CanGoBack);
            if (CanGoForward != oldCanGoForward)
                OnCanGoForwardChanged(CanGoForward);
        }

        private void InsertSourceInternal(int index, Type sourceType, object source, object parameter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (index < 0 || index > this.sources.Count)
                throw new IndexOutOfRangeException();

            this.sources.Insert(index, source);
            this.entries.Insert(index, new NavigationEntry(sourceType, parameter));
        }

        private void InsertAndSelectSourceInternal(int index, Type sourceType, object source, object parameter)
        {
            bool oldCanGoBack = CanGoBack;
            bool oldCanGoForward = CanGoForward;

            InsertSourceInternal(index, sourceType, source, parameter);

            if (currentIndex >= index)
                SetCurrentIndex(currentIndex + 1);

            CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
        }

        internal object InsertNewSource(int index, Type sourceType, object parameter, Func<Type, object> resolveSource)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (resolveSource == null)
                throw new ArgumentNullException(nameof(resolveSource));

            var source = resolveSource(sourceType);
            InsertAndSelectSourceInternal(index, sourceType, source, parameter);

            return source;
        }

        /// <summary>
        /// Inserts a new source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The source created</returns>
        public object InsertNewSource(int index, Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            var source = NavigationHelper.ResolveSource(sourceType);
            InsertAndSelectSourceInternal(index, sourceType, source, parameter);

            return source;
        }

        internal object AddNewSource(Type sourceType, object parameter, Func<Type, object> resolveSource)
        {
            return this.InsertNewSource(this.sources.Count, sourceType, parameter, resolveSource);
        }

        /// <summary>
        /// Adds a new source.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The source created</returns>
        public object AddNewSource(Type sourceType, object parameter)
        {
            return this.InsertNewSource(this.sources.Count, sourceType, parameter);
        }

        /// <summary>
        /// Adds a new source.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <returns>The source created</returns>
        public object AddNewSource(Type sourceType)
        {
            return this.InsertNewSource(this.sources.Count, sourceType, null);
        }

        private void RemoveAtInternal(int index)
        {
            if (index < 0 || index > this.sources.Count - 1)
                throw new IndexOutOfRangeException();

            this.sources.RemoveAt(index);
            this.entries.RemoveAt(index);
        }

        /// <summary>
        /// Removes the source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveSourceAt(int index)
        {
            bool oldCanGoBack = CanGoBack;
            bool oldCanGoForward = CanGoForward;

            RemoveAtInternal(index);

            if (this.sources.Count == 0)
            {
                this.SetCurrentIndex(-1);
                this.SetCurrent(null);
            }
            else if (currentIndex > index)
                this.SetCurrentIndex(currentIndex - 1);

            CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
        }

        /// <summary>
        /// Removes the source.
        /// </summary>
        /// <param name="source">The source</param>
        /// <returns>True if removed</returns>
        public bool RemoveSource(object source)
        {
            int index = sources.IndexOf(source);
            if (index != -1)
            {
                RemoveSourceAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the sources from the index to the end.
        /// </summary>
        /// <param name="startIndex">The start index</param>
        public void RemoveSources(int startIndex)
        {
            if (startIndex >= 0 && startIndex < this.sources.Count)
            {
                bool oldCanGoBack = CanGoBack;
                bool oldCanGoForward = CanGoForward;

                while (sources.Count > startIndex)
                    this.RemoveAtInternal(startIndex);

                if (currentIndex >= startIndex)
                {
                    // select last
                    if (this.sources.Count == 0)
                    {
                        this.SetCurrentIndex(-1);
                        this.SetCurrent(null);
                    }
                    else
                    {
                        int newIndex = startIndex - 1;
                        this.SetCurrentIndex(newIndex);
                        this.SetCurrent(this.sources[newIndex]);
                    }
                }

                CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
            }
        }

        /// <summary>
        /// Clears the source collection.
        /// </summary>
        public void ClearSources()
        {
            bool oldCanGoBack = CanGoBack;
            bool oldCanGoForward = CanGoForward;

            this.sources.Clear();
            this.entries.Clear();
            this.SetCurrentIndex(-1);
            this.SetCurrent(null);

            CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
        }

        #endregion // Sources management

        /// <summary>
        /// Sets the <see cref="Current"/> value with the new value. Can be override to apply other behaviors.
        /// </summary>
        /// <param name="source">The source</param>
        protected virtual void SetCurrent(object source)
        {
            current = source;
            OnPropertyChanged(nameof(Current));
            OnCurrentChanged();
        }

        /// <summary>
        /// Clears the content.
        /// </summary>
        public void ClearContent()
        {
            this.SetCurrent(null);
        }

        private void SyncOnNavigate(Type sourceType, object source, bool isSelectable, NavigationContext navigationContext)
        {
            bool oldCanGoBack = CanGoBack;
            bool oldCanGoForward = CanGoForward;
            if (isSelectable)
            {
                int index = sources.IndexOf(source);
                UpdateEntryParameter(index, navigationContext.Parameter);
                this.SetCurrentIndex(index);
                this.SetCurrent(source);
            }
            else
            {
                var newIndex = this.currentIndex + 1;
                this.InsertSourceInternal(newIndex, sourceType, source, navigationContext.Parameter);
                if (clearSourcesOnNavigate)
                    this.RemoveSources(newIndex + 1);
                UpdateEntryParameter(newIndex, navigationContext.Parameter);
                this.SetCurrentIndex(newIndex);
                this.SetCurrent(source);
            }
            CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
        }

        internal void Navigate(Type sourceType, object parameter, Func<Type, object> resolveSource)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.Navigate(current, sources, navigationContext, resolveSource,
                (source, isSelectable) => SyncOnNavigate(sourceType, source, isSelectable, navigationContext),
                success => OnNavigationCompleted(success, sourceType, parameter, NavigationType.New));
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

            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.Navigate(current, sources, navigationContext,
                (source, isSelectable) => SyncOnNavigate(sourceType, source, isSelectable, navigationContext),
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
            var index = this.currentIndex;
            if (index >= 0)
            {
                this.RemoveAtInternal(index);
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
            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.NavigateFast(current, sources, navigationContext,
                (source, isSelectable) => SyncOnNavigate(sourceType, source, isSelectable, navigationContext));

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
        /// Navigates to the source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            if (index < 0 || index > this.sources.Count - 1)
                throw new IndexOutOfRangeException();

            var navigationType = this.currentIndex > index ? NavigationType.Back : NavigationType.Forward;
            this.MoveInternal(index, navigationType);
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
        /// Navigates to last source.
        /// </summary>
        public void MoveToLast()
        {
            if (this.sources.Count > 0 && currentIndex < this.sources.Count)
                this.MoveTo(this.sources.Count - 1);
        }

        /// <summary>
        /// Navigates to the previous source.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
                this.MoveInternal(this.currentIndex - 1, NavigationType.Back);
        }

        /// <summary>
        /// Navigates to the next source.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward)
                this.MoveInternal(this.currentIndex + 1, NavigationType.Forward);
        }

        /// <summary>
        /// Navigates to the first source.
        /// </summary>
        public void NavigateToRoot()
        {
            if (this.sources.Count > 0)
            {
                this.OnNavigating(this.entries[currentIndex], NavigationType.Root);

                var newIndex = 0;
                var entry = this.entries[newIndex];
                var source = this.sources[newIndex];

                var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
                NavigationHelper.Replace(current, source, navigationContext, () =>
                {
                    bool oldCanGoBack = CanGoBack;
                    bool oldCanGoForward = CanGoForward;

                    if (clearSourcesOnNavigate)
                        RemoveSources(newIndex + 1);

                    UpdateEntryParameter(newIndex, navigationContext.Parameter);
                    this.SetCurrentIndex(newIndex);
                    this.SetCurrent(source);

                    CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
                },
                 success => OnNavigationCompleted(success, navigationContext.SourceType, navigationContext.Parameter, NavigationType.Root));
            }
        }

        private void MoveInternal(int newIndex, NavigationType navigationType)
        {
            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], navigationType);

            var entry = this.entries[newIndex];
            var source = this.sources[newIndex];

            var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
            NavigationHelper.Replace(current, source, navigationContext, () =>
            {
                bool oldCanGoBack = CanGoBack;
                bool oldCanGoForward = CanGoForward;

                UpdateEntryParameter(newIndex, navigationContext.Parameter);
                this.SetCurrentIndex(newIndex);
                this.SetCurrent(source);

                CheckCanGoBackAndCanGoForward(oldCanGoBack, oldCanGoForward);
            },
            success => OnNavigationCompleted(success, navigationContext.SourceType, navigationContext.Parameter, navigationType));
        }


        /// <summary>
        /// Synchronizes the history and sources with the history provided.
        /// </summary>
        public void Sync(IEnumerable<NavigationEntry> entries, IEnumerable<object> sources, int currentIndex)
        {
            this.ClearSources();

            object currentSource = null;
            object currentParameter = null;
            int index = 0;
            foreach (var entry in entries)
            {
                var originalSource = sources.ElementAt(index);
                var source = NavigationHelper.EnsureNewView(originalSource);

                this.InsertSourceInternal(index, entry.SourceType, source, entry.Parameter);

                if (index == currentIndex)
                {
                    currentSource = source;
                    currentParameter = entry.Parameter;
                }
                index++;
            }

            if (currentIndex != -1)
            {
                this.UpdateEntryParameter(currentIndex, currentParameter);
                this.SetCurrentIndex(currentIndex);
                this.SetCurrent(currentSource);

                CheckCanGoBackAndCanGoForward(false, false);
            }
        }
    }
}
