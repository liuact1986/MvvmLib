using MvvmLib.Mvvm;

namespace NavigationSample.Wpf.ViewModels
{
    public class TransitioningControlViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public TransitioningControlViewModel()
        {
            Message = "My Awesome Message (ViewModel Content  + DataTemplate)";
        }
    }
}
