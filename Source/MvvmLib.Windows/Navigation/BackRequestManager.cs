using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to show the title bar back button.
    /// </summary>
    public class BackRequestManager : IBackRequestManager
    {
        protected Frame frame;
        protected Action navigationActionCallback;

        /// <summary>
        /// Handles SystemNavigationManager back requested.
        /// </summary>
        /// <param name="frame">The frame</param>
        /// <param name="navigationActionCallback">Go back callback</param>
        public virtual void Handle(Frame frame, Action navigationActionCallback)
        {
            this.frame = frame;
            this.navigationActionCallback = navigationActionCallback;

            this.frame.RegisterPropertyChangedCallback(Frame.CanGoBackProperty, OnCanGoBackChanged);

            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            this.CheckVisibility();
        }

        /// <summary>
        /// Unhandles SystemNavigationManager back requested.
        /// </summary>
        public virtual void Unhandle()
        {
            this.navigationActionCallback = null;

            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;

            HideBackButton();
        }

        #region Back button Visibility management

        protected virtual void ShowBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }

        protected virtual void HideBackButton()
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        protected void CheckVisibility()
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

        protected virtual void OnCanGoBackChanged(DependencyObject sender, DependencyProperty dp)
        {
            this.CheckVisibility();
        }

        #endregion // Back button Visibility management


        // action to call (navigate)

        protected virtual void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (frame.CanGoBack)
            {
                navigationActionCallback?.Invoke();
                e.Handled = true;
            }
        }
    }

}
