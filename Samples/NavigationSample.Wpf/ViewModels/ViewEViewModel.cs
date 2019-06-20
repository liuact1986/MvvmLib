using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;
using System.Collections.Generic;
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

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            Message = navigationContext.Parameter != null ? navigationContext.Parameter.ToString() : "Welcome Message";
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void CanActivate(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            if (!IsLoggedIn())
            {
                continuationCallback(false);
                var navigationParameters = new Dictionary<string, object>
                {
                    { "redirectTo",typeof(ViewE) },
                    { "parameter", navigationContext.Parameter }
                };
                Navigation.NavigateFast(typeof(LoginView), navigationParameters);
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


        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            var navigationParameters = navigationContext.Parameter as Dictionary<string, object>;
            this.redirectTo = (Type)navigationParameters["redirectTo"];
            this.parameter = navigationParameters["parameter"];
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
