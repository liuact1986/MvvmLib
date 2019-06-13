using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        public ICommand LoginCommand { get; }
        private INavigationManager navigationManager;

        public LoginPageViewModel(INavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;

            LoginCommand = new RelayCommand(() =>
            {
                User.IsLoggedIn = true;
                navigationManager.GetDefault().PushAsync(typeof(HomePage), "Your are logged in.");
            });
        }
    }

}
