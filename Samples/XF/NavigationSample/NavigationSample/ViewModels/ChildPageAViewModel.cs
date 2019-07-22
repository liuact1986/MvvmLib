using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace NavigationSample.ViewModels
{
    

    public class ChildPageAViewModel : BindableBase, INavigatable, IDeactivatable
    {

        private string message = "My default Child PageA message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand NavigateCommand { get; }

        public ChildPageAViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            NavigateCommand = new DelegateCommand(() =>
            {
                navigationManager.GetNamed("tabNav").PushAsync(typeof(ChildPageB), "Child PageB message");
            });
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate Child PageA?", "Ok", "Cancel");
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

        }

        public void OnNavigatingFrom()
        {

        }
    }

}
