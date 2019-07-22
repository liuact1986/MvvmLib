using MvvmLib.Commands;
using System;
using System.Collections.Generic;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Container for <see cref="NavigationSource"/>. Allows to navigate with multiple navigation source simultaneously.
    /// </summary>
    public class NavigationSourceContainer
    {
        private readonly List<NavigationSource> navigationSources;
        /// <summary>
        /// The navigation sources.
        /// </summary>
        public IReadOnlyList<NavigationSource> NavigationSources
        {
            get { return navigationSources; }
        }

        /// <summary>
        /// Gets the navigation source at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns></returns>
        public NavigationSource this[int index]
        {
            get
            {
                if (index < 0 || index > this.navigationSources.Count - 1)
                    throw new IndexOutOfRangeException();

                return navigationSources[index];
            }
        }

        /// <summary>
        /// The count of <see cref="NavigationSource"/> for the container.
        /// </summary>
        public int Count
        {
            get { return this.navigationSources.Count; }
        }

        private readonly IDelegateCommand navigateCommand;
        /// <summary>
        /// Allows to navigate to the source with the source type provided for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand NavigateCommand
        {
            get { return navigateCommand; }
        }

        private readonly IDelegateCommand moveToFirstCommand;
        /// <summary>
        /// Allows to move to the first source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToFirstCommand
        {
            get { return moveToFirstCommand; }
        }

        private readonly IDelegateCommand moveToPreviousCommand;
        /// <summary>
        /// Allows to move to the previous source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToPreviousCommand
        {
            get { return moveToPreviousCommand; }
        }

        private readonly IDelegateCommand moveToNextCommand;
        /// <summary>
        /// Allows to move to the next source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToNextCommand
        {
            get { return moveToNextCommand; }
        }

        private readonly IDelegateCommand moveToLastCommand;
        /// <summary>
        /// Allows to move to the last source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToLastCommand
        {
            get { return moveToLastCommand; }
        }

        private readonly IDelegateCommand moveToIndexCommand;
        /// <summary>
        /// Allows to move to the index for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToIndexCommand
        {
            get { return moveToIndexCommand; }
        }

        private readonly IDelegateCommand moveToCommand;
        /// <summary>
        /// Allows to move to the source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IDelegateCommand MoveToCommand
        {
            get { return moveToCommand; }
        }


        /// <summary>
        /// Creates the <see cref="NavigationSourceContainer"/>.
        /// </summary>
        public NavigationSourceContainer()
        {
            navigationSources = new List<NavigationSource>();

            navigateCommand = new DelegateCommand<Type>(ExecuteNavigateCommand);
            moveToFirstCommand = new DelegateCommand(ExecuteMoveToFirstCommand);
            moveToPreviousCommand = new DelegateCommand(ExecuteMoveToPreviousCommand);
            moveToNextCommand = new DelegateCommand(ExecuteMoveToNextCommand);
            moveToLastCommand = new DelegateCommand(ExecuteMoveToLastCommand);
            moveToIndexCommand = new DelegateCommand<object>(ExecuteMoveToIndexCommand);
            moveToCommand = new DelegateCommand<object>(ExecuteMoveToCommand);
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

        private void ExecuteMoveToPreviousCommand()
        {
            MoveToPrevious();
        }


        private void ExecuteMoveToNextCommand()
        {
            MoveToNext();
        }

        private void ExecuteMoveToLastCommand()
        {
            MoveToLast();
        }


        private void ExecuteMoveToIndexCommand(object args)
        {
            MoveTo(args);
        }

        private void ExecuteMoveToCommand(object args)
        {
            MoveTo(args);
        }

        #endregion // Commands

        #region Collection management

        /// <summary>
        /// Adds the <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        public void Register(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (navigationSources.Count > 0)
            {
                navigationSource.Sync(navigationSources[0]);
            }

            this.navigationSources.Add(navigationSource);

        }

        /// <summary>
        /// Checks if the navigation source is registered.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        /// <returns>True if registered</returns>
        public bool IsRegistered(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            return this.navigationSources.Contains(navigationSource);
        }

        /// <summary>
        /// Removes the <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        public bool Unregister(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (this.navigationSources.Contains(navigationSource))
            {
                return this.navigationSources.Remove(navigationSource);
            }
            return false;
        }

        /// <summary>
        /// Clears the <see cref="NavigationSources"/>.
        /// </summary>
        public void UnregisterAll()
        {
            this.navigationSources.Clear();
        }

        #endregion // Collection management  

        private static object ResolveSource(Type sourceType, ref object sharedContext)
        {
            if (NavigationHelper.IsFrameworkElementType(sourceType))
            {
                var source = NavigationHelper.CreateNew(sourceType);
                var view = source as FrameworkElement;
                if (view.DataContext == null)
                {
                    if (sharedContext == null)
                        sharedContext = NavigationHelper.ResolveViewModelWithViewModelLocator(sourceType);

                    view.DataContext = sharedContext;
                }
                return source;
            }
            else
            {
                if (sharedContext == null)
                    sharedContext = NavigationHelper.CreateNew(sourceType);

                return sharedContext;
            }
        }

        /// <summary>
        /// Inserts a new source at the index for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void InsertNewSource(int index, Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            object sharedContext = null;
            foreach (var navigationSource in navigationSources)
                navigationSource.InsertNewSource(index, sourceType, parameter, (s) => ResolveSource(s, ref sharedContext));
        }

        /// <summary>
        /// Inserts new source for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="sourceType">The source type</param>
        public void InsertNewSource(int index, Type sourceType)
        {
            this.InsertNewSource(index, sourceType, null);
        }

        /// <summary>
        /// Adds a new source for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void AddNewSource(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            object sharedContext = null;
            foreach (var navigationSource in navigationSources)
                navigationSource.AddNewSource(sourceType, parameter, (s) => ResolveSource(s, ref sharedContext));
        }

        /// <summary>
        /// Adds a new source for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void AddNewSource(Type sourceType)
        {
            this.AddNewSource(sourceType, null);
        }

        /// <summary>
        /// Removes source at the index and history for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveSourceAt(int index)
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.RemoveSourceAt(index);
        }

        /// <summary>
        /// Clears sources and history for all <see cref="NavigationSources"/>.
        /// </summary>
        public void ClearSources()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.ClearSources();
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/> for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            object sharedContext = null;
            foreach (var navigationSource in navigationSources)
                navigationSource.Navigate(sourceType, parameter, (s) => ResolveSource(s, ref sharedContext));
        }

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/> for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void Navigate(Type sourceType)
        {
            this.Navigate(sourceType, null);
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Redirect(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            foreach (var navigationSource in navigationSources)
                navigationSource.Redirect(sourceType, parameter);
        }

        /// <summary>
        /// Redirects and remove the previous entry from the history for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        public void Redirect(Type sourceType)
        {
            this.Redirect(sourceType, null);
        }

        /// <summary>
        /// Navigates to the first source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void MoveToFirst()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.MoveToFirst();
        }

        /// <summary>
        /// Navigates to the previous source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void MoveToPrevious()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.MoveToPrevious();
        }

        /// <summary>
        /// Navigates to the next source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void MoveToNext()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.MoveToNext();
        }

        /// <summary>
        /// Navigates to last source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void MoveToLast()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.MoveToLast();
        }

        /// <summary>
        /// Navigates to the source at the index for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            foreach (var navigationSource in navigationSources)
            {
                if (index >= 0 && index < navigationSource.Sources.Count)
                    navigationSource.MoveTo(index);
            }
        }

        /// <summary>
        /// Navigates to the specified source for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="source">The source</param>
        public void MoveTo(object source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var navigationSource in navigationSources)
                navigationSource.MoveTo(source);
        }
    }
}
