using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    public class ModalPageBViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable, INavigationParameterKnowledge
    {

        public object Parameter { get; set; }

        private string message = "My default Modal PageB message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand GoBackCommand { get; }

        public ModalPageBViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            GoBackCommand = new DelegateCommand(() =>
            {
                navigationManager.GetDefault().PopModalAsync("GoBack Modal message");
            });
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            var result = await dialogService.DisplayAlertAsync("Activate?", "Activate ModalPageB?", "Ok", "Cancel");
            return result;
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate ModalPageB?", "Ok", "Cancel");
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
