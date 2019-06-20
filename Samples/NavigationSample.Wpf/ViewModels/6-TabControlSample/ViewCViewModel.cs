using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{

    public class ViewCViewModel : ViewModelBase, INavigationAware, IIsSelected, IDetailViewModel
    {
        public ViewCViewModel()
        { }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                if (isSelected)
                    Message = "SELECTED";
                else
                    Message = "NOT Selected";
            }
        }

        public void OnNavigatingFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatingTo(NavigationContext navigationContext)
        {
            if (navigationContext.Parameter != null)
                Message = (string)navigationContext.Parameter;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
