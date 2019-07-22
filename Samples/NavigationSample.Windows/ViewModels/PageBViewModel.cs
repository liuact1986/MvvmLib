using MvvmLib.Commands;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using Windows.UI.Xaml.Navigation;

namespace NavigationSample.Windows.ViewModels
{
    public class PageBViewModel : BindableBase, INavigatable
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

        public PageBViewModel()
        {
            message = "Default PageB message";

            UpdateMessageCommand = new DelegateCommand(() =>
            {
                Message += "!";
            });
        }

        public void OnNavigatedTo(object parameter, NavigationMode navigationMode)
        {
            if (parameter != null)
            {
                Message = (string)parameter;
            }
            Count++;
        }

        public void OnNavigatingFrom(bool isSuspending)
        {

        }
    }
}
