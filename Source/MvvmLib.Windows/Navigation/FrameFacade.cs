using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// A facade for the frame content control.
    /// </summary>
    public class FrameFacade : IFrameFacade
    {
        Frame frame;

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
        /// Gets the value that indicates if the can go back.
        /// </summary>
        public bool CanGoBack => frame.CanGoBack;


        /// <summary>
        /// Gets the value that indicates if the can go forward.
        /// </summary>
        public bool CanGoForward => frame.CanGoForward;

        /// <summary>
        /// Gets the frame content.
        /// </summary>
        public object Content => frame.Content;

        /// <summary>
        /// Creates the frame facade.
        /// </summary>
        /// <param name="frame">The frame control</param>
        public FrameFacade(Frame frame)
        {
            this.frame = frame;

            this.frame.Navigating += OnFrameNavigating;
            this.frame.Navigated += OnFrameNavigated;

            this.frame.RegisterPropertyChangedCallback(Frame.CanGoBackProperty, OnCanGoBackChanged);
            this.frame.RegisterPropertyChangedCallback(Frame.CanGoForwardProperty, OnCanGoForwardChanged);
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            this.Navigating?.Invoke(this, new FrameNavigatingEventArgs(e.SourcePageType, e.Parameter, e.NavigationMode));
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            this.Navigated?.Invoke(this, new FrameNavigatedEventArgs(e.SourcePageType, e.Content, e.Parameter, e.NavigationMode));
        }

        private void OnCanGoBackChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.CanGoBackChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnCanGoForwardChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.CanGoForwardChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        public void Navigate(Type sourcePageType)
        {
            frame.Navigate(sourcePageType);
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        public void Navigate(Type sourcePageType, object parameter)
        {
            frame.Navigate(sourcePageType, parameter);
        }

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="infoOverride">The navigation transition</param>
        public void Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            frame.Navigate(sourcePageType, parameter, infoOverride);
        }

        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        public void GoBack()
        {
            frame.GoBack();
        }


        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        /// <param name="infoOverride">The navigation transition</param>
        public void GoBack(NavigationTransitionInfo infoOverride)
        {
            frame.GoBack(infoOverride);
        }


        /// <summary>
        /// Navigates to next entry.
        /// </summary>
        public void GoForward()
        {
            frame.GoForward();
        }


        /// <summary>
        /// Returns the navigation state string for App life cycle.
        /// </summary>
        /// <returns>The navigation state string</returns>
        public string GetNavigationState()
        {
            return frame.GetNavigationState();
        }


        /// <summary>
        /// Restore the navigation with the navigation state.
        /// </summary>
        /// <param name="navigationState">The navigation state</param>
        public void SetNavigationState(string navigationState)
        {
            frame.SetNavigationState(navigationState);
        }
    }
}
