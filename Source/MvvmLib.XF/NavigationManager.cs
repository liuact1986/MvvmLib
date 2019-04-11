using System.Collections.Generic;
using Xamarin.Forms;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to manage multiple navigation services of the application.
    /// </summary>
    public class NavigationManager : INavigationManager
    {
        private const string DefaultNavigationPageName = "__default__";

        static Dictionary<string, INavigationService> navigationServices = new Dictionary<string, INavigationService>();

        public static INavigationService Register(INavigationService navigationService, string name)
        {
            navigationServices[name] = navigationService;
            return navigationService;
        }

        public static INavigationService Register(INavigationService navigationService)
        {
            return Register(navigationService, DefaultNavigationPageName);
        }

        public static INavigationService Register(NavigationPage navigationPage, string name)
        {
            var navigationStrategy = new NavigationPageFacade(navigationPage);
            var navigationService = new PageNavigationService(navigationStrategy);
            return Register(navigationService, name);
        }

        public static INavigationService Register(NavigationPage navigationPage)
        {
            return Register(navigationPage, DefaultNavigationPageName);
        }

        public static bool Unregister(string name)
        {
            if (navigationServices.ContainsKey(name))
            {
                navigationServices.Remove(name);
                return true;
            }
            return false;
        }

        public static bool UnregisterDefault()
        {
            return Unregister(DefaultNavigationPageName);
        }

        public static bool IsRegistered(string name)
        {
            return navigationServices.ContainsKey(name);
        }

        public static bool IsRegistered()
        {
            return IsRegistered(DefaultNavigationPageName);
        }

        public INavigationService GetNamed(string name)
        {
            if (!IsRegistered(name)) { throw new NavigationException("No navigation service with the name \"" + name + "\" registered"); }

            return navigationServices[name];
        }

        public INavigationService GetDefault()
        {
            return GetNamed(DefaultNavigationPageName);
        }

    }
}
