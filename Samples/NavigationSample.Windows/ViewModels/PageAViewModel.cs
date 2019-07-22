using System.Threading.Tasks;
using MvvmLib.Mvvm;
using NavigationSample.Windows.Services;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml.Controls;
using System.Linq;
using MvvmLib.Navigation;
using JsonLib;
using MvvmLib.Commands;

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

        public DelegateCommand UpdateMessageCommand { get; }

        public PageAViewModel(IMyService myService)
        {
            message = myService.GetMessage("PageAViewModel");

            UpdateMessageCommand = new DelegateCommand(() =>
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
