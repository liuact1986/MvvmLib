using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewDViewModel : ViewModelBase, ISelectable, IDetailViewModel, INavigationAware
    {
        public ViewDViewModel()
        {
            Message = "View D [ViewModel]";
        }

        public bool IsTarget(Type sourceType, object parameter)
        {
            // return sourceType == typeof(IDetailViewModel);
            return sourceType == typeof(ViewD) || sourceType == typeof(ViewDViewModel);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameter != null)
                Message = navigationContext.Parameter.ToString();
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {

        }
    }
}
