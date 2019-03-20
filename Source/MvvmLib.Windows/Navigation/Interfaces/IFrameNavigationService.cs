﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation service contract.
    /// </summary>
    public interface IFrameNavigationService
    {
        /// <summary>
        /// Gets the value that indicates if the can go back.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Gets the value that indicates if the can go forward.
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        /// Invoked when the can go back value changed.
        /// </summary>
        event EventHandler CanGoBackChanged;

        /// <summary>
        /// Invoked when can the go forward value changed.
        /// </summary>
        event EventHandler CanGoForwardChanged;

        /// <summary>
        /// Invoked after page navigated. 
        /// </summary>
        event EventHandler<FrameNavigatedEventArgs> Navigated;

        /// <summary>
        /// Invoked before page navigation.
        /// </summary>
        event EventHandler<FrameNavigatingEventArgs> Navigating;


        /// <summary>
        /// Invoked when the navigation is canceled.
        /// </summary>
        event EventHandler<FrameNavigationCanceledEventArgs> NavigationCanceled;

        /// <summary>
        /// Returns the navigation state string for App life cycle.
        /// </summary>
        /// <returns>The navigation state string</returns>
        string GetNavigationState();

        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        /// <returns></returns>
        Task GoBackAsync();

        /// <summary>
        /// Navigates to previous entry.
        /// </summary>
        /// <param name="infoOverride">The navigation transition</param>
        /// <returns></returns>
        Task GoBackAsync(NavigationTransitionInfo infoOverride);

        /// <summary>
        /// Navigates to next entry.
        /// </summary>
        /// <returns></returns>
        Task GoForwardAsync();

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <returns></returns>
        Task NavigateAsync(Type sourcePageType);

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns></returns>
        Task NavigateAsync(Type sourcePageType, object parameter);

        /// <summary>
        /// Navigates to the page.
        /// </summary>
        /// <param name="sourcePageType">The page type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="infoOverride">The navigation transition</param>
        /// <returns></returns>
        Task NavigateAsync(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride);

        /// <summary>
        /// Restore the navigation with the navigation state.
        /// </summary>
        /// <param name="navigationState">The navigation state</param>
        void SetNavigationState(string navigationState);

        /// <summary>
        /// A method to call on application suspension to save states from current view model.
        /// </summary>
        void Suspend();
    }
}