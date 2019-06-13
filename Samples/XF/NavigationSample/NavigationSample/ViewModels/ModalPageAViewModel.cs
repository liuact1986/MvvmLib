using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NavigationSample.ViewModels
{
    public class ModalPageAViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable, INavigationParameterKnowledge
    {

        public object Parameter { get; set; }

        private string message = "My default Modal PageA message";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private IDialogService dialogService;
        private INavigationManager navigationManager;

        public ICommand NavigateCommand { get; }

        public ModalPageAViewModel(IDialogService dialogService, INavigationManager navigationManager)
        {
            this.dialogService = dialogService;
            this.navigationManager = navigationManager;

            NavigateCommand = new RelayCommand(() =>
            {
                navigationManager.GetDefault().PushModalAsync(typeof(ModalPageB), "Modal PageB message", true);
            });
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            var result = await dialogService.DisplayAlertAsync("Activate?", "Activate ModalPageA?", "Ok", "Cancel");
            return result;
        }

        public async Task<bool> CanDeactivateAsync()
        {
            var result = await dialogService.DisplayAlertAsync("Deactivate?", "Deactivate ModalPageA?", "Ok", "Cancel");
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
