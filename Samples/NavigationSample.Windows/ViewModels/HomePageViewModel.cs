using MvvmLib.Mvvm;
using Windows.UI.Xaml.Navigation;
using NavigationSample.Windows.Views;
using MvvmLib.Navigation;
using System.Collections.Generic;
using NavigationSample.Windows.Services;

namespace NavigationSample.Windows.ViewModels
{
    public class HomePageViewModel : BindableBase, INavigatable
    {
        private string message = "Default HomePage message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private INavigationManager navigationManager;

        public HomePageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        public async void OnNavigatedTo(object parameter, NavigationMode navigationMode)
        {
            if (User.IsLoggedIn)
            {
                if (parameter != null)
                {
                    Message = parameter.ToString();
                }
            }
            else
            {
                var navigationParameters = new Dictionary<string, object>
                {
                    { "redirectTo",typeof(HomePage).FullName },
                    { "parameter", parameter }
                };
                var queryString = QueryHelper.ToQueryString(navigationParameters);
                await navigationManager.GetDefault().RedirectAsync(typeof(LoginPage), queryString);
            }
        }

        public void OnNavigatingFrom(bool isSuspending)
        {

        }
    }
}
