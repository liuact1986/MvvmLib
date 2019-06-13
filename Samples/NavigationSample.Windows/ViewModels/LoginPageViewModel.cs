using MvvmLib.Mvvm;
using Windows.UI.Xaml.Navigation;
using System;
using System.Windows.Input;
using MvvmLib.Navigation;
using NavigationSample.Windows.Services;
using MvvmLib.Commands;

namespace NavigationSample.Windows.ViewModels
{
    public class User
    {
        public static bool IsLoggedIn { get; set; }
    }

    public class LoginPageViewModel : INavigatable
    {
        public ICommand LoginCommand { get; }

        private Type redirectToViewType;
        private object parameter;
        private INavigationManager navigationManager;


        public LoginPageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            LoginCommand = new RelayCommand(async () =>
            {
                User.IsLoggedIn = true;
                await navigationManager.GetDefault().RedirectAsync(redirectToViewType, parameter);
            });
        }

        public void OnNavigatedTo(object parameter, NavigationMode navigationMode)
        {
            var parameters = QueryHelper.FromQueryString(parameter.ToString());
            this.redirectToViewType = Type.GetType(parameters["redirectTo"].ToString());
            this.parameter = parameters["parameter"];
        }

        public void OnNavigatingFrom(bool isSuspending)
        {

        }
    }
}
