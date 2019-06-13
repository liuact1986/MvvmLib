using MvvmLib.Mvvm;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewAViewModel : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewAViewModel()
        {
            Message = "View A [ViewModel]";
        }
    }
}
