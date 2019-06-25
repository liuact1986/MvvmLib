using MvvmLib.Commands;
using MvvmLib.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The navigation source base class.
    /// </summary>
    public class NavigationSource : IMovableSource, INotifyPropertyChanged
    {
        private readonly ILogger DefaultLogger = new DebugLogger();

        private ILogger logger;
        /// <summary>
        /// The logger used.
        /// </summary>
        public ILogger Logger
        {
            get { return logger ?? DefaultLogger; }
            set { logger = value; }
        }

        private ObservableCollection<object> sources;
        /// <summary>
        /// The sources collection.
        /// </summary>
        public IReadOnlyCollection<object> Sources
        {
            get { return sources; }
        }

        private readonly List<NavigationEntry> entries;
        /// <summary>
        /// The entry collection.
        /// </summary>
        public IReadOnlyList<NavigationEntry> Entries
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
            set
            {
                if (!Equals(currentIndex, value))
                    MoveTo(value);
            }
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

        /// <summary>
        /// Checks if can move to previous source.
        /// </summary>
        public bool CanMoveToPrevious
        {
            get { return this.currentIndex > 0; }
        }

        /// <summary>
        /// Checks if can move to next source.
        /// </summary>
        public bool CanMoveToNext
        {
            get { return this.currentIndex < this.sources.Count - 1; }
        }

        private IRelayCommand navigateCommand;
        /// <summary>
        /// Allows to navigate to the source with the source type provided.
        /// </summary>
        public IRelayCommand NavigateCommand
        {
            get
            {
                if (navigateCommand == null)
                    navigateCommand = new RelayCommand<Type>(ExecuteNavigateCommand);
                return navigateCommand;
            }
        }

        private IRelayCommand moveToFirstCommand;
        /// <summary>
        /// Allows to move to the first source.
        /// </summary>
        public IRelayCommand MoveToFirstCommand
        {
            get
            {
                if (moveToFirstCommand == null)
                    moveToFirstCommand = new RelayCommand(ExecuteMoveToFirstCommand, CanExecuteMoveToFirstCommand);
                return moveToFirstCommand;
            }
        }

        private IRelayCommand moveToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous source.
        /// </summary>
        public IRelayCommand MoveToPreviousCommand
        {
            get
            {
                if (moveToPreviousCommand == null)
                    moveToPreviousCommand = new RelayCommand(ExecuteMoveToPreviousCommand, CanExecuteMoveToPreviousCommand);
                return moveToPreviousCommand;
            }
        }

        private IRelayCommand moveToNextCommand;
        /// <summary>
        /// Allows to move to the next source.
        /// </summary>
        public IRelayCommand MoveToNextCommand
        {
            get
            {
                if (moveToNextCommand == null)
                    moveToNextCommand = new RelayCommand(ExecuteMoveToNextCommand, CanExecuteMoveToNextCommand);
                return moveToNextCommand;
            }
        }

        private IRelayCommand moveToLastCommand;
        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        public IRelayCommand MoveToLastCommand
        {
            get
            {
                if (moveToLastCommand == null)
                    moveToLastCommand = new RelayCommand(ExecuteMoveToLastCommand, CanExecuteMoveToLastCommand);
                return moveToLastCommand;
            }
        }

        private IRelayCommand moveToIndexCommand;
        /// <summary>
        /// Allows to move to the index.
        /// </summary>
        public IRelayCommand MoveToIndexCommand
        {
            get
            {
                if (moveToIndexCommand == null)
                    moveToIndexCommand = new RelayCommand<object>(ExecuteMoveToIndexCommand);
                return moveToIndexCommand;
            }
        }

        private IRelayCommand moveToCommand;
        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        public IRelayCommand MoveToCommand
        {
            get
            {
                if (moveToCommand == null)
                    moveToCommand = new RelayCommand<object>(ExecuteMoveToCommand);
                return moveToCommand;
            }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on <see cref="CanMoveToPrevious"/> changed.
        /// </summary>
        public event EventHandler<CanMoveToEventArgs> CanMoveToPreviousChanged;

        /// <summary>
        /// Invoked on <see cref="CanMoveToNext"/> changed.
        /// </summary>
        public event EventHandler<CanMoveToEventArgs> CanMoveToNextChanged;

        /// <summary>
        /// Invoked before navigation starts.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigating;

        /// <summary>
        /// Invoked after navigation ends.
        /// </summary>
        public event EventHandler<NavigationEventArgs> Navigated;

        /// <summary>
        /// Invoked on navigation failed (cancelled or exception).
        /// </summary>
        public event EventHandler<NavigationEventArgs> NavigationFailed;

        /// <summary>
        /// Invoked on current source changed.
        /// </summary>
        public event EventHandler<CurrentSourceChangedEventArgs> CurrentChanged;

        /// <summary>
        /// Creates the <see cref="NavigationSource"/>.
        /// </summary>
        public NavigationSource()
        {
            this.sources = new ObservableCollection<object>();
            this.entries = new List<NavigationEntry>();
            this.clearSourcesOnNavigate = true;
            this.currentIndex = -1;
            this.notifyOnCurrentChanged = true;
        }

        #region Commands

        private void ExecuteNavigateCommand(Type sourceType)
        {
            this.Navigate(sourceType, null);
        }

        private void ExecuteMoveToFirstCommand()
        {
            MoveToFirst();
        }

        private bool CanExecuteMoveToFirstCommand()
        {
            return CanMoveToPrevious;
        }

        private void ExecuteMoveToPreviousCommand()
        {
            MoveToPrevious();
        }

        private bool CanExecuteMoveToPreviousCommand()
        {
            return CanMoveToPrevious;
        }

        private void ExecuteMoveToNextCommand()
        {
            MoveToNext();
        }

        private bool CanExecuteMoveToNextCommand()
        {
            return CanMoveToNext;
        }

        private void ExecuteMoveToLastCommand()
        {
            MoveToLast();
        }

        private bool CanExecuteMoveToLastCommand()
        {
            return CanMoveToNext;
        }

        private void ExecuteMoveToIndexCommand(object args)
        {
            if (args != null)
            {
                if (int.TryParse(args.ToString(), out int position))
                {
                    MoveTo(position);
                }
            }
        }

        private void ExecuteMoveToCommand(object args)
        {
            MoveTo(args);
        }

        #endregion // Commands

        #region Events

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCanMoveToPreviousChanged(bool canMove)
        {
            moveToFirstCommand?.RaiseCanExecuteChanged();
            moveToPreviousCommand?.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanMoveToPrevious));
            CanMoveToPreviousChanged?.Invoke(this, new CanMoveToEventArgs(canMove));
        }

        private void OnCanMoveToNextChanged(bool canMove)
        {
            moveToLastCommand?.RaiseCanExecuteChanged();
            moveToNextCommand?.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanMoveToNext));
            CanMoveToNextChanged?.Invoke(this, new CanMoveToEventArgs(canMove));
        }

        private void OnNavigating(NavigationEntry entry, NavigationType navigationType)
        {
            Navigating?.Invoke(this, new NavigationEventArgs(entry.SourceType, entry.Parameter, navigationType));
        }

        private void OnNavigated(Type sourceType, object parameter, NavigationType navigationType)
        {
            Navigated?.Invoke(this, new NavigationEventArgs(sourceType, parameter, navigationType));
        }

        private void OnNavigationFailed(Type sourceType, object parameter, NavigationType navigationType)
        {
            NavigationFailed?.Invoke(this, new NavigationEventArgs(sourceType, parameter, navigationType));
        }

        private void OnNavigationCompleted(bool success, Type sourceType, object parameter, NavigationType navigationType)
        {
            if (success)
                this.OnNavigated(sourceType, parameter, navigationType);
            else
                this.OnNavigationFailed(sourceType, parameter, navigationType);
        }

        private void OnCurrentChanged()
        {
            CurrentChanged?.Invoke(this, new CurrentSourceChangedEventArgs(this.currentIndex, this.current));
        }

        #endregion // Events

        #region Sources management

        private void SetCurrentIndex(int index)
        {
            currentIndex = index;
            if (notifyOnCurrentChanged)
                OnPropertyChanged(nameof(CurrentIndex));
        }

        private void UpdateEntryParameter(int index, object parameter)
        {
            if (index >= 0)
                this.entries[index].Parameter = parameter;
        }

        private void CheckCanMoveTo(bool oldCanMoveToPrevious, bool oldCanMoveToNext)
        {
            if (CanMoveToPrevious != oldCanMoveToPrevious)
                OnCanMoveToPreviousChanged(CanMoveToPrevious);
            if (CanMoveToNext != oldCanMoveToNext)
                OnCanMoveToNextChanged(CanMoveToNext);
        }

        private void InsertSourceInternal(int index, Type sourceType, object source, object parameter)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (index < 0 || index > this.sources.Count)
                throw new IndexOutOfRangeException();

            this.entries.Insert(index, new NavigationEntry(sourceType, parameter));
            this.sources.Insert(index, source);
        }


        private bool notifyOnCurrentChanged;
        /// <summary>
        /// Allows to cancel <see cref="INotifyPropertyChanged"/>. Usefull sometimes with animations.
        /// </summary>
        public bool NotifyOnCurrentChanged
        {
            get { return notifyOnCurrentChanged; }
            set { notifyOnCurrentChanged = value; }
        }

        private void InsertSourceAndSetCurrentIndex(int index, Type sourceType, object source, object parameter)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            InsertSourceInternal(index, sourceType, source, parameter);

            if (currentIndex >= index)
                SetCurrentIndex(currentIndex + 1);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        internal object InsertNewSource(int index, Type sourceType, object parameter, Func<Type, object> resolveSource)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));
            if (resolveSource == null)
                throw new ArgumentNullException(nameof(resolveSource));

            var source = resolveSource(sourceType);
            InsertSourceAndSetCurrentIndex(index, sourceType, source, parameter);

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
            InsertSourceAndSetCurrentIndex(index, sourceType, source, parameter);

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

            this.entries.RemoveAt(index);
            this.sources.RemoveAt(index);
        }

        /// <summary>
        /// Removes the source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveSourceAt(int index)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            RemoveAtInternal(index);

            if (this.sources.Count == 0)
            {
                this.SetCurrentIndex(-1);
                this.SetCurrent(null);
            }
            else if (this.currentIndex == index)
            {
                // remove selected
                // (1) [A B] xC [D E] => [A B] D [E] (old 2, new 2 + change current)
                // (1) xA [B C] => B [C] (old 0, new 0 + change current)
                // (else) [A B] xC => [A] B (old 2, new 1 + change current)
                if (this.sources.Count > index)
                    this.SetCurrent(this.sources[index]);
                else
                {
                    var newIndex = index - 1;
                    this.SetCurrentIndex(newIndex);
                    this.SetCurrent(this.sources[newIndex]);
                }
            }
            else if (currentIndex > index)
            {
                // [xA B] C [D E] => [B] C [D E] change current index - 1 (old 2, new 1)
                // [A B] C [D xE] => [A B] C [D] not change
                this.SetCurrentIndex(currentIndex - 1);
            }

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
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
                var oldCanMoveToPrevious = CanMoveToPrevious;
                var oldCanMoveToNext = CanMoveToNext;

                while (sources.Count > startIndex)
                    this.RemoveAtInternal(startIndex);

                // A [B xC xD xE] => A [B] (old 0, new 0) not change
                // [A xB] xC [xD xE] => A => select last
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

                CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
            }
        }

        /// <summary>
        /// Clears the source collection.
        /// </summary>
        public void ClearSources()
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            this.sources.Clear();
            this.entries.Clear();
            this.SetCurrentIndex(-1);
            this.SetCurrent(null);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);
        }

        #endregion // Sources management

        /// <summary>
        /// Sets the <see cref="Current"/> value with the new value. Can be override to apply other behaviors.
        /// </summary>
        /// <param name="source">The source</param>
        protected virtual void SetCurrent(object source)
        {
            current = source;
            if (notifyOnCurrentChanged)
            {
                OnPropertyChanged(nameof(Current));
                OnCurrentChanged();
            }
        }

        /// <summary>
        /// Clears the content.
        /// </summary>
        public void ClearContent()
        {
            this.SetCurrent(null);
        }

        private void ExecuteNavigationWithSelectable(object selectable, NavigationContext navigationContext)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            NavigationHelper.OnNavigatingFrom(current, navigationContext);

            int index = sources.IndexOf(selectable);
            UpdateEntryParameter(index, navigationContext.Parameter);
            this.SetCurrentIndex(index);
            this.SetCurrent(selectable);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);

            this.OnNavigated(navigationContext.SourceType, navigationContext.Parameter, NavigationType.Back);
        }

        private void ExecuteNavigationNew(object source, object context, NavigationContext navigationContext)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            NavigationHelper.OnNavigatingFrom(current, navigationContext);

            NavigationHelper.OnNavigatingTo(context, navigationContext);

            var newIndex = this.currentIndex + 1;
            this.InsertSourceInternal(newIndex, navigationContext.SourceType, source, navigationContext.Parameter);

            if (clearSourcesOnNavigate)
                this.RemoveSources(newIndex + 1);

            UpdateEntryParameter(newIndex, navigationContext.Parameter);
            this.SetCurrentIndex(newIndex);
            this.SetCurrent(source);

            NavigationHelper.OnNavigatedTo(context, navigationContext);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);

            this.OnNavigated(navigationContext.SourceType, navigationContext.Parameter, NavigationType.New);
        }

        internal void Navigate(Type sourceType, object parameter, Func<Type, object> resolveSource)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);
            NavigationHelper.CanDeactivate(current, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                {
                    var selectable = NavigationHelper.FindSelectable(sources, sourceType, parameter);
                    if (selectable != null)
                    {
                        NavigationHelper.CanActivate(selectable, navigationContext, canActivate =>
                        {
                            if (canActivate)
                                ExecuteNavigationWithSelectable(selectable, navigationContext);
                            else
                                this.OnNavigationFailed(navigationContext.SourceType, navigationContext.Parameter, NavigationType.New);
                        });
                    }
                    else
                    {
                        var source = resolveSource(sourceType);
                        NavigationHelper.CanActivate(source, navigationContext, canActivate =>
                        {
                            if (canActivate)
                            {
                                var view = source as FrameworkElement;
                                if (view != null)
                                    ExecuteNavigationNew(source, view.DataContext, navigationContext);
                                else
                                    ExecuteNavigationNew(source, source, navigationContext);
                            }
                            else
                                this.OnNavigationFailed(navigationContext.SourceType, navigationContext.Parameter, NavigationType.New);
                        });
                    }
                }
                else
                    this.OnNavigationFailed(navigationContext.SourceType, navigationContext.Parameter, NavigationType.New);
            });
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(Type sourceType, object parameter)
        {
            this.Navigate(sourceType, parameter, NavigationHelper.ResolveSource);
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

            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], NavigationType.New);

            var navigationContext = new NavigationContext(sourceType, parameter);

            var selectable = NavigationHelper.FindSelectable(sources, sourceType, parameter);
            if (selectable != null)
                ExecuteNavigationWithSelectable(selectable, navigationContext);
            else
            {
                var source = NavigationHelper.ResolveSource(sourceType);
                var view = source as FrameworkElement;
                if (view != null)
                    ExecuteNavigationNew(source, view.DataContext, navigationContext);
                else
                    ExecuteNavigationNew(source, source, navigationContext);
            }
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

        #region Move

        private void MoveToInternal(int index, NavigationType navigationType)
        {
            if (this.currentIndex >= 0)
                this.OnNavigating(this.entries[currentIndex], navigationType);

            var entry = this.entries[index];
            var source = this.sources[index];

            var navigationContext = new NavigationContext(entry.SourceType, entry.Parameter);
            NavigationHelper.CanDeactivate(current, navigationContext, canDeactivate =>
            {
                if (canDeactivate)
                {
                    NavigationHelper.CanActivate(source, navigationContext, canActivate =>
                    {
                        if (canActivate)
                        {
                            var view = source as FrameworkElement;
                            if (view != null)
                                ExecuteMove(index, source, view.DataContext, navigationContext, navigationType);
                            else
                                ExecuteMove(index, source, source, navigationContext, navigationType);
                        }
                        else
                            this.OnNavigationFailed(navigationContext.SourceType, navigationContext.Parameter, navigationType);
                    });
                }
                else
                    this.OnNavigationFailed(navigationContext.SourceType, navigationContext.Parameter, navigationType);
            });
        }

        private void ExecuteMove(int index, object source, object context, NavigationContext navigationContext, NavigationType navigationType)
        {
            var oldCanMoveToPrevious = CanMoveToPrevious;
            var oldCanMoveToNext = CanMoveToNext;

            NavigationHelper.OnNavigatingFrom(current, navigationContext);

            NavigationHelper.OnNavigatingTo(context, navigationContext);

            if (clearSourcesOnNavigate && navigationType == NavigationType.Root)
                RemoveSources(index + 1);

            UpdateEntryParameter(index, navigationContext.Parameter);
            this.SetCurrentIndex(index);
            this.SetCurrent(source);

            NavigationHelper.OnNavigatedTo(context, navigationContext);

            CheckCanMoveTo(oldCanMoveToPrevious, oldCanMoveToNext);

            this.OnNavigated(navigationContext.SourceType, navigationContext.Parameter, navigationType);
        }

        /// <summary>
        /// Allows to move to the first source.
        /// </summary>
        public void MoveToFirst()
        {
            if (CanMoveToPrevious)
                this.MoveToInternal(0, NavigationType.Root);
        }

        /// <summary>
        /// Allows to move to the previous source.
        /// </summary>
        public void MoveToPrevious()
        {
            if (CanMoveToPrevious)
                this.MoveToInternal(this.currentIndex - 1, NavigationType.Back);
        }

        /// <summary>
        /// Allows to move to the next source.
        /// </summary>
        public void MoveToNext()
        {
            if (CanMoveToNext)
                this.MoveToInternal(this.currentIndex + 1, NavigationType.Forward);
        }

        /// <summary>
        /// Allows to move to the last source.
        /// </summary>
        public void MoveToLast()
        {
            if (CanMoveToNext)
                this.MoveToInternal(this.sources.Count - 1, NavigationType.Forward);
        }

        /// <summary>
        /// Allows to move to the source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            if (index >= 0 && index < this.sources.Count)
            {
                var navigationType = this.currentIndex > index ? NavigationType.Back : NavigationType.Forward;
                MoveToInternal(index, navigationType);
            }
        }

        /// <summary>
        /// Allows to move to the source.
        /// </summary>
        /// <param name="source">The source</param>
        public void MoveTo(object source)
        {
            var index = this.sources.IndexOf(source);
            this.MoveTo(index);
        }

        #endregion // Move

        /// <summary>
        /// Synchronizes the navigation source with the navigation source provided.
        /// </summary>
        public void Sync(NavigationSource navigationSource)
        {
            this.ClearSources();

            var entries = navigationSource.Entries;
            var sources = navigationSource.Sources;
            var currentIndex = navigationSource.CurrentIndex;

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

                CheckCanMoveTo(false, false);
            }
        }
    }
}
