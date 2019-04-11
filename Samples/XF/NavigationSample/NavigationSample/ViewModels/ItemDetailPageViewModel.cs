using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using Xamarin.Forms;

namespace NavigationSample.ViewModels
{
    public class ItemDetailPageViewModel : BindableBase, INavigatable
    {
        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private Item item;
        public Item Item
        {
            get { return item; }
            set { SetProperty(ref item, value); }
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {

        }

        public void OnNavigatingTo(object parameter)
        {
            if (parameter != null && parameter is Item p)
            {
                Item = p;
                Title = p.Name;
            }
        }
    }
}
