using System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to show the title bar back button.
    /// </summary>
    public class BackRequestManager : IBackRequestManager
    {
        Frame frame;
        Action callback;

        /// <summary>
        /// Handles SystemNavigationManager back requested.
        /// </summary>
        /// <param name="frame">The frame</param>
        /// <param name="callback">Go back callback</param>
        public void Handle(Frame frame, Action callback)
        {
            this.frame = frame;
            this.callback = callback;

            this.frame.Navigated += OnNavigated;
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (this.frame.CanGoBack)
            {
                ShowBackButton();
            }
        }

        /// <summary>
        /// Unhandles SystemNavigationManager back requested.
        /// </summary>
        public void Unhandle()
        {
            this.callback = null;

            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
            this.frame.Navigated -= OnNavigated;

            HideBackButton();
        }

        private void ShowBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        private void HideBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (frame.CanGoBack)
            {
                callback?.Invoke();
                e.Handled = true;
            }
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (frame.CanGoBack)
            {
                ShowBackButton();
            }
            else
            {
                HideBackButton();
            }
        }
    }

}
