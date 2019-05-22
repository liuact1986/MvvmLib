using MvvmLib.Mvvm;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitoningControlViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public TransitoningControlViewModel()
        {
            Message = "My Awesome Message (ViewModel Content  + DataTemplate)";
        }
    }
}
