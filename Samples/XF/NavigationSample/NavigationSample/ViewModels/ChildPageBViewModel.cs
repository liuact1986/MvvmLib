using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    public class ChildPageBViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable
    {

        private string message = "My default Child PageB message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand GoBackCommand { get; }

        public ChildPageBViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            GoBackCommand = new RelayCommand(() =>
            {
                navigationManager.GetNamed("tabNav").PopAsync("My Child GoBack message", true);
            });
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            var result = await dialogService.DisplayAlertAsync("Activate?", "Activate Child PageB?", "Ok", "Cancel");
            return result;
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate Child PageB?", "Ok", "Cancel");
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
