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

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameter != null)
                Message = navigationContext.Parameter.ToString();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }
    }
}
