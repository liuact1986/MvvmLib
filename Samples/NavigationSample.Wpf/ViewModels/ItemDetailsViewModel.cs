using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace NavigationSample.Wpf.ViewModels
{
    public class ItemDetailsViewModel : BindableBase, INavigatable
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }


        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null)
                Name = (string)parameter;
        }
    }

}
