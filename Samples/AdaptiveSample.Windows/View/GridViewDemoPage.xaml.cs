using MvvmLib.Commands;
using MvvmLib.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace AdaptiveSample.Windows
{
    public sealed partial class GridViewDemoPage : Page
    {
        public GridViewDemoPage()
        {
            this.InitializeComponent();
            var items = GetItems();
            Items = new ObservableCollection<Item>(items);

            this.SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 960)
            {
                NormalGridView.Columns = 4;
            }
            else if (e.NewSize.Width > 600)
            {
                NormalGridView.Columns = 2;
                ResponsiveGridView.Columns = 3;
            }
            else
            {
                NormalGridView.Columns = 1;
                ResponsiveGridView.Columns = 2;
            }
        }

        public List<Item> GetItems()
        {
            return new List<Item>()
            {
                new Item { Color="DarkRed", Title="Item 1" },
                new Item { Color="DarkGray", Title="Item 2" },
                new Item { Color="Brown", Title="Item 3" },
                new Item { Color="DarkGreen", Title="Item 4" },
                new Item { Color="DarkOrange", Title="Item 5" },
                new Item { Color="DarkCyan", Title="Item 6" },
            };
        }

        public ObservableCollection<Item> Items { get; set; }

        public ICommand Command
        {
            get
            {
                return new RelayCommand<object>(async (r) =>
                {
                    var dialog = new MessageDialog("Clicked on : " + ((Item)r).Title);
                    await dialog.ShowAsync();
                });
            }
        }
    }


}
