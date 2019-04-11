using MvvmLib.IoC;
using MvvmLib.Navigation;
using NavigationSample.Views;
using Xamarin.Forms;

namespace NavigationSample
{
    public class Bootstrapper : MvvmLibBootstrapper
    {
        public Bootstrapper(IInjector container)
            :base(container)
        { }

        protected override void ConfigureNavigation(Page shell)
        {
            var navigationService = NavigationManager.Register((NavigationPage)shell);

            //navigationService.StoreActivePages = false;

            // if check activation is required
            navigationService.PushAsync(typeof(HomePage), "My Home Page message", true);
        }

        protected override Page CreateShell()
        {
            //return new NavigationPage(new HomePage());
            return new NavigationPage();
        }

        protected override void InitializeShell(Page shell)
        {
            App.Current.MainPage = shell;
        }

        protected override void RegisterTypes()
        {
            container.RegisterSingleton<INavigationManager, NavigationManager>();
        }

    }
}
