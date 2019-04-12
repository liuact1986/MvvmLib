using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The main navigation service implementation. Allows to navigate, cancel navigation and notify view models with parameter.
    /// </summary>
    public class FrameNavigationService : INavigationService
    {
        IFrameFacade frameFacade;

        /// <summary>
        /// Invoked after page navigated. 
        /// </summary>
        public event EventHandler<FrameNavigatedEventArgs> Navigated;

        /// <summary>
        /// Invoked before page navigation.
        /// </summary>
        public event EventHandler<FrameNavigatingEventArgs> Navigating;

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        public event EventHandler CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        public event EventHandler CanGoForwardChanged;

        /// <summary>
        /// Invoked when the navigation is canceled.
        /// </summary>
        public event EventHandler<FrameNavigationCanceledEventArgs> NavigationCanceled;

        /// <summary>
        /// Gets the value that indicates if the frame can go back.
        /// </summary>
        public bool CanGoBack => frameFacade.CanGoBack;

        /// <summary>
        /// Gets the value that indicates if the frame can go forward.
        /// </summary>
        public bool CanGoForward => frameFacade.CanGoForward;

        /// <summary>
        /// The frame back stack.
        /// </summary>
        public IList<PageStackEntry> BackStack => frameFacade.BackStack;

        /// <summary>
        /// The frame forward stack.
        /// </summary>
        public IList<PageStackEntry> ForwardStack => frameFacade.ForwardStack;

        /// <summary>
        /// Creates the navigation service.
        /// </summary>
        /// <param name="frameFacade">The frame facade</param>
        public FrameNavigationService(IFrameFacade frameFacade)
        {
            this.frameFacade = frameFacade;

            this.frameFacade.CanGoBackChanged += OnCanGoBackChanged; ;
            this.frameFacade.CanGoForwardChanged += OnCanGoForwardChanged;

            this.frameFacade.Navigating += OnFrameFacadeNavigating;
            this.frameFacade.Navigated += OnFrameFacadeNavigated;
        }

        private void OnCanGoBackChanged(object sender, EventArgs e)
        {
            this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCanGoForwardChanged(object sender, EventArgs e)
        {
            this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnFrameFacadeNavigating(object sender, FrameNavigatingEventArgs e)
        {
            // on navigating from current viewmodel
            this.DoNavigatingFromCurrentViewModel(false);
            this.Navigating?.Invoke(this, new FrameNavigatingEventArgs(e.SourcePageType, e.Parameter, e.NavigationMode));
        }

        private void OnFrameFacadeNavigated(object sender, FrameNavigatedEventArgs e)
        {
            // on navigated to new view model
            this.DoNavigatedToViewModel(e.Content, e.Parameter, e.NavigationMode);
            this.Navigated?.Invoke(this, new FrameNavigatedEventArgs(e.SourcePageType, e.Content, e.Parameter, e.NavigationMode));
        }

        private void DoNavigatingFromCurrentViewModel(bool suspending)
        {
            var view = this.frameFacade.Content as FrameworkElement;
            if (view != null)
            {
                var viewModel = view.DataContext as INavigatable;
                if (viewModel != null)
                {
                    viewModel.OnNavigatingFrom(suspending);
                }
            }
        }

        private void DoNavigatedToViewModel(object content, object parameter, NavigationMode navigationMode)
        {
            if (content != null)
            {
                var view = content as FrameworkElement;
                if (view != null)
                {
                    var viewModel = view.DataContext as INavigatable;
                    if (viewModel != null)
                    {
                        viewModel.OnNavigatedTo(parameter, navigationMode);
                    }
                }
            }
        }

        private async Task<bool> CanDeactivateAsync()
        {
            var view = this.frameFacade.Content as FrameworkElement;
            if (view != null)
            {
                var viewModel = view.DataContext;
                if (viewModel != null && viewModel is IDeactivatable)
                {
                    var canDeactivate = await ((IDeactivatable)viewModel).CanDeactivateAsync();
                    return canDeactivate;
                }
            }
            return true;
        }

        private async Task PerformNavigationAsync(Action callback)
        {
            if (await CanDeactivateAsync())
            {
                callback();
            }
            else
            {
                this.NavigationCanceled?.Invoke(this, new FrameNavigationCanceledEventArgs(this.frameFacade.Content));
            }
        }

        /// <summary>
        /// Returns the navigation state string for App life cycle.
        /// </summary>
        /// <returns>The navigation state string</returns>
        public string GetNavigationState()
        {
            return frameFacade.GetNavigationState();
        }

        /// <summary>
        /// Restore the navigation with the navigation state.
        /// </summary>
        /// <param name="navigationState">The navigation state</param>
        public void SetNavigationState(string navigationState)
        {
            frameFacade.SetNavigationState(navigationState);
        }

        /// <summary>
        /// A method to call on application suspension to save states from current view model.
        /// </summary>
        public void Suspend()
        {
            DoNavigatingFromCurrentViewModel(true);
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourcePageType)
        {
            await PerformNavigationAsync(() => frameFacade.Navigate(sourcePageType));
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourcePageType, object parameter)
        {
            await PerformNavigationAsync(() => frameFacade.Navigate(sourcePageType, parameter));
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="infoOverride">The navigation transition</param>
        /// <returns></returns>
        public async Task NavigateAsync(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            await PerformNavigationAsync(() => frameFacade.Navigate(sourcePageType, parameter, infoOverride));
        }


        /// <summary>
        /// Redirect to the page and remove the previous page from history.
        /// </summary>
        /// <param name="sourcePageType">The type of the page to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="infoOverride">The navigation transition</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            var currentSourcePageType = frameFacade.CurrentSourcePageType;

            // delay 
            await Task.Delay(1);

            await PerformNavigationAsync(() => frameFacade.Navigate(sourcePageType, parameter, infoOverride));

            // remove page from history
            var entry = BackStack.LastOrDefault();
            if(entry != null && entry.SourcePageType == currentSourcePageType)
            {
                BackStack.Remove(entry);
            }
        }

        /// <summary>
        /// Redirect to the page and remove the previous page from history.
        /// </summary>
        /// <param name="sourcePageType">The type of the page to redirect</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourcePageType, object parameter)
        {
            await RedirectAsync(sourcePageType, parameter, null);
        }

        /// <summary>
        /// Redirect to the page and remove the previous page from history.
        /// </summary>
        /// <param name="sourcePageType">The type of the page to redirect</param>
        /// <returns></returns>
        public async Task RedirectAsync(Type sourcePageType)
        {
            await RedirectAsync(sourcePageType, null, null);
        }

        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        /// <returns></returns>
        public async Task GoBackAsync()
        {
            await PerformNavigationAsync(() => frameFacade.GoBack());
        }


        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        /// <param name="infoOverride">The navigation transition</param>
        /// <returns></returns>
        public async Task GoBackAsync(NavigationTransitionInfo infoOverride)
        {
            await PerformNavigationAsync(() => frameFacade.GoBack(infoOverride));
        }

        /// <summary>
        /// Navigates to next entry.
        /// </summary>
        /// <returns></returns>
        public async Task GoForwardAsync()
        {
            await PerformNavigationAsync(() => frameFacade.GoForward());
        }

    }
}
