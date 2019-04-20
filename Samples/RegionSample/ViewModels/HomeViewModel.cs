using System.Threading.Tasks;
using RegionSample.Views;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Collections.Generic;

namespace RegionSample.ViewModels
{
    public class User
    {
        public static bool IsLoggedIn { get; set; }
    }

    public class HomeViewModel : BindableBase, IActivatable, INavigatable
    {
        private string message = "Default HomePage message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IRegionNavigationService navigationService;

        private ContentRegion contentRegion;

        public HomeViewModel(IRegionNavigationService navigationService)
        {
            this.navigationService = navigationService;

            contentRegion = navigationService.GetContentRegion("ContentRegion", "ContentRegion1");
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            if (User.IsLoggedIn)
            {
                return true;
            }
            else
            {
                var navigationParameters = new Dictionary<string, object>
                {
                    { "redirectTo",typeof(HomeView) },
                    { "parameter", parameter }
                };
                // redirect remove current page (if present) from history
                await contentRegion.RedirectAsync(typeof(LoginView), navigationParameters);
                return false;
            }
        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
            {
                Message = parameter.ToString();
            }
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {

        }
    }
}
