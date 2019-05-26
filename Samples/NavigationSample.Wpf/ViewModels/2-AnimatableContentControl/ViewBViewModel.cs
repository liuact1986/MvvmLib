using MvvmLib.Mvvm;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewBViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewBViewModel()
        {
            Message = "View B [ViewModel]";
        }
    }
}
