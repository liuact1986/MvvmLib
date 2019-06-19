using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class User
    {
        public static bool IsLoggedIn { get; set; }
    }

    public class ViewEViewModel : BindableBase, INavigationAware, ICanActivate
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public NavigationSource Navigation { get; }

        public ViewEViewModel()
        {
           this.Navigation = NavigationManager.GetDefaultNavigationSource("HistorySample");
        }


        private bool IsLoggedIn()
        {
            return User.IsLoggedIn;
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            Message = parameter != null ? parameter.ToString() : "Welcome Message";
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void CanActivate(object parameter, Action<bool> continuationCallback)
        {
            if (!IsLoggedIn())
            {
                continuationCallback(false);
                var navigationParameters = new Dictionary<string, object>
                {
                    { "redirectTo",typeof(ViewE) },
                    { "parameter", parameter }
                };
                Navigation.EndWith(typeof(LoginView), navigationParameters);
            }
            else
            {
                continuationCallback(true);
            }
        }
    }

    public class LoginViewModel : INavigationAware
    {
        private Type redirectTo;
        private object parameter;

        public NavigationSource Navigation { get; }
        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            this.Navigation = NavigationManager.GetDefaultNavigationSource("HistorySample");
            LoginCommand = new RelayCommand(Login);
        }

        private void Login()
        {
            User.IsLoggedIn = true;
            // redirect (removes the login view from the history)
            Navigation.Redirect(redirectTo, parameter);
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            var navigationParameters = parameter as Dictionary<string, object>;
            this.redirectTo = (Type)navigationParameters["redirectTo"];
            this.parameter = navigationParameters["parameter"];
        }

        public void OnNavigatedTo(object parameter)
        {

        }
    }
}
