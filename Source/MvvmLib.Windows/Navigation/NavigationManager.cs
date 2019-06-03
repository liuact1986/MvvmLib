using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to manage multiple navigation services of the application.
    /// </summary>
    public class NavigationManager : INavigationManager
    {
        private const string DefaultFrameName = "__default__";

        private static Dictionary<string, KeyValuePair<Frame, INavigationService>> navigationServices
            = new Dictionary<string, KeyValuePair<Frame, INavigationService>>();

        /// <summary>
        /// registers the frame with the name, creates and returns a navigation service.
        /// </summary>
        /// <param name="frame">The frame</param>
        /// <param name="name">The name</param>
        /// <returns>The navigation service</returns>
        public static INavigationService Register(Frame frame, string name)
        {
            var navigationService = new FrameNavigationService(new FrameFacade(frame));
            navigationServices[name] = new KeyValuePair<Frame, INavigationService>(frame, navigationService);
            return navigationService;
        }

        /// <summary>
        /// registers the frame with the default name, creates and returns a navigation service.
        /// </summary>
        /// <param name="frame">The frame</param>
        /// <returns>The navigation service</returns>
        public static INavigationService Register(Frame frame)
        {
            return Register(frame, DefaultFrameName);
        }

        /// <summary>
        /// Remove the navigation service.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>True if service is removed</returns>
        public static bool Unregister(string name)
        {
            return navigationServices.Remove(name);
        }

        /// <summary>
        /// Remove the navigation service.
        /// </summary>
        /// <returns>True if service is removed</returns>
        public static bool UnregisterDefault()
        {
            return Unregister(DefaultFrameName);
        }

        /// <summary>
        /// Checks if the named navigation service is registered.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>True if registered</returns>
        public static bool IsRegistered(string name)
        {
            return navigationServices.ContainsKey(name);
        }

        /// <summary>
        /// Checks if the default navigation service is registered.
        /// </summary>
        /// <returns>True if registered</returns>
        public static bool IsRegistered()
        {
            return IsRegistered(DefaultFrameName);
        }


        /// <summary>
        /// Returns the named navigation service.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The navigation service</returns>
        public INavigationService GetNamed(string name)
        {
            if (!IsRegistered(name))
                throw new NavigationException("No navigation service with the name \"" + name + "\" registered"); 

            return navigationServices[name].Value;
        }

        /// <summary>
        /// Returns the default navigation service.
        /// </summary>
        /// <returns>The navigation service</returns>
        public INavigationService GetDefault()
        {
            return GetNamed(DefaultFrameName);
        }

    }

}
