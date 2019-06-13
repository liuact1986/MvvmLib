using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace HelloWorld.ViewModels
{
    public class HomeViewModel : BindableBase, INavigationAware
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
                Message = parameter.ToString();
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {

        }
    }
}
