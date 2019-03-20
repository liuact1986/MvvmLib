using System.Threading.Tasks;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Windows.Services;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml.Controls;

namespace NavigationSample.Windows.ViewModels
{
    public class PageAViewModel : BindableBase, INavigatable, IDeactivatable
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        public RelayCommand UpdateMessageCommand { get; }

        public PageAViewModel(IMyService myService)
        {
            message = myService.GetMessage("PageAViewModel");

            UpdateMessageCommand = new RelayCommand(() =>
            {
                Message += "!";
            });
        }

        public void OnNavigatedTo(object parameter, NavigationMode navigationMode)
        {
            Count++;
        }

        public void OnNavigatingFrom(bool isSuspending)
        {

        }

        public async Task<bool> CanDeactivateAsync()
        {
            bool result = true;

            var dialog = new MessageDialog("Deactivate PageA?");
            dialog.Commands.Add(new UICommand("Ok", cmd => { }));
            dialog.Commands.Add(new UICommand("Cancel", cmd => { result = false; }));

            await dialog.ShowAsync();

            return result;
        }
    }
}
