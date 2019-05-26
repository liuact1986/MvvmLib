using MvvmLib.Navigation;
using NavigationSample.Wpf.Views;
using System;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewDViewModel : ViewModelBase, ISelectable, IDetailViewModel
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
    }
}
