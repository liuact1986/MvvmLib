using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewCViewModel : ViewModelBase, INavigatable, IIsSelected
    {
        public ViewCViewModel(IRegionNavigationService regionNavigationService)
            : base(regionNavigationService)
        { }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                SetProperty(ref isSelected, value);
                if (isSelected)
                    Message = "ACTIVE";
                else
                    Message = "NOT Active";
            }
        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
                Message = (string)parameter;
        }

        public void OnNavigatedTo(object parameter)
        {

        }
    }
}
