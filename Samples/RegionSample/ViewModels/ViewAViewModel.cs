using RegionSample.Views;
using System;
using System.Threading.Tasks;
using System.Windows;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{

    public class ViewAViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable, ILoadedEventListener
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private string loadedMessage;
        public string LoadedMessage
        {
            get { return loadedMessage; }
            set { SetProperty(ref loadedMessage, value); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        private RelayCommand updateMessageCommand;
        public RelayCommand UpdateMessageCommand =>
            updateMessageCommand ?? (updateMessageCommand = new RelayCommand(ExecuteUpdateMessageCommand));

        IRegionManager regionManager;

        public ViewAViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            message = "Initial ViewA message";
            count = 0;
        }

        void ExecuteUpdateMessageCommand()
        {
            Message += "!";
        }

        public async Task<bool> CanActivateAsync(object parameter)
        {
            //var result = MessageBox.Show("Activate View A?", "Activate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            //return Task.FromResult(result);

            if (MessageBox.Show("Activate View A?", "Activate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                return true;
            }
            else
            {
                // redirect
                await regionManager.GetContentRegion("ContentRegion","ContentRegion1").NavigateAsync(typeof(LoginView));
                return false;
            }
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Deactivate View A?", "Deactivate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public void OnNavigatingTo(object parameter)
        {
            Count++;
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnLoaded(FrameworkElement view, object parameter)
        {
            LoadedMessage = "Loaded with parameter : " + parameter?.ToString() + ", " + DateTime.Now.ToLongTimeString();
        }

        public void OnNavigatingFrom()
        {
            
        }
    }
}
