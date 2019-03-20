using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace HelloWorld.ViewModels
{
    public class HomeViewModel : BindableBase, INavigatable
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public void OnNavigatedTo(object parameter)
        {
            if (parameter != null)
            {
                Message = parameter.ToString();
            }
        }

        public void OnNavigatingFrom()
        {
           
        }
    }
}
