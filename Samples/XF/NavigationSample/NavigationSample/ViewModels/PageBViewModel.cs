using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NavigationSample.ViewModels
{
    public class MyTabbedPageViewModel : IPageKnowledge
    {

        public void GetPage(Page page)
        {
            var myTabbedPage = page as MyTabbedPage;

            NavigationManager.Register(myTabbedPage.tabNav, "tabNav");

            myTabbedPage.Disappearing += (s, e) =>
            {
                NavigationManager.Unregister("tabNav");
            };
        }

    }

    public class PageBViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable
    {

        private string message = "My default PageB message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string myText;
        public string MyText
        {
            get { return myText; }
            set { SetProperty(ref myText, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand GoBackCommand { get; }
        public ICommand NavigateCommand { get; }

        public PageBViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            GoBackCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PopAsync("My GoBack message", true);
            });

            NavigateCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PopToRootAsync("My RootPage message", true);
            });
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            var result = await dialogService.DisplayAlertAsync("Activate?", "Activate PageB?", "Ok", "Cancel");
            return result;
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate PageB?", "Ok", "Cancel");
            return result;
        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
            {
                Message = parameter.ToString();
            }

            MyText = "";
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {

        }
    }

}
