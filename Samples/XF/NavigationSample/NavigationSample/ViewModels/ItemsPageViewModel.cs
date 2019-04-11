using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using NavigationSample.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace NavigationSample.ViewModels
{
    public class Item
    {
        public string Name { get; set; }
    }

    public class ItemsPageViewModel : BindableBase
    {
        string title = "Browse";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ObservableCollection<Item> Items { get; set; }

        public ICommand SelectCommand { get; }

        public ItemsPageViewModel(INavigationManager navigationManager)
        {
            Items = new ObservableCollection<Item>
            {
                new Item{Name="Item A"},
                new Item{Name="Item B"}
            };

            SelectCommand = new RelayCommand<SelectedItemChangedEventArgs>((t) =>
            {
                navigationManager.GetNamed("MasterDetail").PushAsync(typeof(ItemDetailPage), parameter: t.SelectedItem);
            });
        }
    }
}
