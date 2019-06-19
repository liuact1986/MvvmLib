using MvvmLib.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// Checks if can go back.
        /// </summary>
        public bool CanGoBack
        {
            get { return this.navigationSources.Count > 0 && this.navigationSources[0].History.CanGoBack; }
        }

        /// <summary>
        /// Checks if can go forward. 
        /// </summary>
        public bool CanGoForward
        {
            get { return this.navigationSources.Count > 0 && this.navigationSources[0].History.CanGoForward; }
        }

        private readonly IRelayCommand navigateCommand;
        /// <summary>
        /// Allows to navigate to the source with the source type provided for all <see cref="NavigationSources"/>.
        /// </summary>
        public IRelayCommand NavigateCommand
        {
            get { return navigateCommand; }
        }

        private readonly IRelayCommand goBackCommand;
        /// <summary>
        /// Allows to navigate to the previous source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IRelayCommand GoBackCommand
        {
            get { return goBackCommand; }
        }

        private readonly IRelayCommand goForwardCommand;
        /// <summary>
        /// Allows to navigate to the next source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IRelayCommand GoForwardCommand
        {
            get { return goForwardCommand; }
        }

        private readonly IRelayCommand navigateToRootCommand;
        /// <summary>
        /// Allows to navigate to the first source for all <see cref="NavigationSources"/>.
        /// </summary>
        public IRelayCommand NavigateToRootCommand
        {
            get { return navigateToRootCommand; }
        }

        private readonly IRelayCommand redirectCommand;
        /// <summary>
        /// Allows to redirect to the source with the source type provided for all <see cref="NavigationSources"/>.
        /// </summary>
        public IRelayCommand RedirectCommand
        {
            get { return redirectCommand; }
        }

        /// <summary>
        /// The count of <see cref="NavigationSource"/> for the container.
        /// </summary>
        public int Count
        {
            get { return this.navigationSources.Count; }
        }

        /// <summary>
        /// Creates the <see cref="NavigationSourceContainer"/>.
        /// </summary>
        public NavigationSourceContainer()
        {
            navigationSources = new List<NavigationSource>();

            navigateCommand = new RelayCommand<Type>(ExecuteNavigateCommand);
            goBackCommand = new RelayCommand(ExecuteGoBackCommand);
            goForwardCommand = new RelayCommand(ExecuteGoForwardCommand);
            navigateToRootCommand = new RelayCommand(ExecuteNavigateToRootCommand);
            redirectCommand = new RelayCommand<Type>(ExecuteRedirectCommand);
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

        private void ExecuteGoForwardCommand()
        {
             this.GoForward();
        }

        private void ExecuteNavigateToRootCommand()
        {
            this.NavigateToRoot();
        }

        #endregion // Commands

        #region Collection management

        /// <summary>
        /// Adds the <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        public void Add(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            if (navigationSources.Count > 0)
            {
                var existingNavigationSource = navigationSources[0];
                navigationSource.Sync(existingNavigationSource.History);
            }

            this.navigationSources.Add(navigationSource);

        }

        /// <summary>
        /// Checks if the navigation source is registered.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        /// <returns>True if registered</returns>
        public bool Contains(NavigationSource navigationSource)
        {
            if (navigationSource == null)
                throw new ArgumentNullException(nameof(navigationSource));

            return this.navigationSources.Contains(navigationSource);
        }

        /// <summary>
        /// Removes the <see cref="NavigationSource"/>.
        /// </summary>
        /// <param name="navigationSource">The navigation source</param>
        public bool Remove(NavigationSource navigationSource)
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
        public void Clear()
        {
            this.navigationSources.Clear();
        }

        #endregion // Collection management  

        /// <summary>
        /// Navigates to the source and notifies ViewModels that implements <see cref="INavigationAware"/> for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(Type sourceType, object parameter)
        {
            if (sourceType == null)
                throw new ArgumentNullException(nameof(sourceType));

            foreach (var navigationSource in navigationSources)
            {
                navigationSource.Navigate(sourceType, parameter);
            }
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
            {
                navigationSource.Redirect(sourceType, parameter);
            }
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
        /// Navigates to the previous source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void GoBack()
        {
            foreach (var navigationSource in navigationSources)
            {
                navigationSource.GoBack();
            }
        }

        /// <summary>
        /// Navigates to the next source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void GoForward()
        {
            foreach (var navigationSource in navigationSources)
            {
                navigationSource.GoForward();
            }
        }

        /// <summary>
        /// Navigates to the first source for all <see cref="NavigationSources"/>.
        /// </summary>
        public void NavigateToRoot()
        {
            foreach (var navigationSource in navigationSources)
            {
                navigationSource.NavigateToRoot();
            }
        }

        /// <summary>
        /// Navigates to the source at the index for all <see cref="NavigationSources"/>.
        /// </summary>
        /// <param name="index">The index</param>
        public void MoveTo(int index)
        {
            foreach (var navigationSource in navigationSources)
            {
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
            {
                navigationSource.MoveTo(source);
            }
        }

        /// <summary>
        /// Clears sources and history for all <see cref="NavigationSources"/>.
        /// </summary>
        public void ClearSources()
        {
            foreach (var navigationSource in navigationSources)
                navigationSource.Clear();
        }
    }
}
