using System;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{

    public class ViewAViewModel : ViewModelBase 
    {
        public ViewAViewModel(IRegionNavigationService regionNavigationService)
            : base(regionNavigationService)
        {
            Message = "View A [ViewModel]";
        }
    }
}
