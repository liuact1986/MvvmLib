using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    /// <summary>
    /// Fake for sample
    /// </summary>
    public class User
    {
        public static bool IsLoggedIn { get; set; }
    }

    public class HomePageViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable
    {

        private string message = "My default HomePage message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand NavigateCommand { get; }
        public ICommand NavigateToTabbedPageCommand { get; }
        public ICommand ShowModalPageCommand { get; }
        public ICommand MasterDetailPageCommand { get; }

        public HomePageViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            NavigateCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PushAsync(typeof(PageA), "PageA message");
            });

            NavigateToTabbedPageCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PushAsync(typeof(MyTabbedPage));
            });

            ShowModalPageCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PushModalAsync(typeof(ModalPageA));
            });

            MasterDetailPageCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PushAsync(typeof(MyMasterDetailPage));
            });
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            if (!User.IsLoggedIn)
            {
                await navigationManager.GetDefault().PushAsync(typeof(LoginPage));
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate HomePage?", "Ok", "Cancel");
            return result;
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
            if (User.IsLoggedIn)
            {
                var page = navigationManager.GetDefault().PreviousPage;
                if (page != null && page is LoginPage p)
                {
                    navigationManager.GetDefault().RemovePage(p);
                }
            }
        }

        public void OnNavigatingFrom()
        {

        }
    }

}
