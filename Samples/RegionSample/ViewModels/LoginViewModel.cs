using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace RegionSample.ViewModels
{
    public class LoginViewModel : INavigatable
    {
        public ICommand LoginCommand { get; }

        private Type redirectToViewType;
        private object parameter;

        public LoginViewModel(IRegionManager regionManager)
        {
            LoginCommand = new RelayCommand(async () =>
            {
                User.IsLoggedIn = true;
                // redirect remove current page (if present) from history
                await regionManager.GetContentRegion("ContentRegion", "ContentRegion1").RedirectAsync(redirectToViewType, parameter);
            });
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            var navigationParameters = parameter as Dictionary<string, object>;
            redirectToViewType = (Type)navigationParameters["redirectTo"];
            this.parameter = navigationParameters["parameter"];
        }

        public void OnNavigatedTo(object parameter)
        {

        }
    }
}
