using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewBViewModel : ViewModelBase 
    {
        public ViewBViewModel(IRegionNavigationService regionNavigationService)
            : base(regionNavigationService)
        {
            Message = "View B [ViewModel]";
        }
    }
}
