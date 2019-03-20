using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmLib.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace AdaptiveSample.Windows
{
    public sealed partial class WithBreakpointPage : Page
    {
        public WithBreakpointPage()
        {

            this.InitializeComponent();

            var items = GetItems();
            Items = new ObservableCollection<Item>(items);

            var variableSizedItems = GetVariableSizedItems();
            VariableSizedItems = new ObservableCollection<VariableItem>(variableSizedItems);

            bp.ActiveChanged += AdaptiveBinding_ActiveChanged; ;
        }

        private void AdaptiveBinding_ActiveChanged(object sender, MvvmLib.Adaptive.ActiveChangedEventArgs e)
        {
            //var columns = (int)e.Active["Columns"];
            //ResponsiveGridView.Columns = columns;
        }

        public List<Item> GetItems()
        {
            var result = new List<Item>()
            {
                new Item { Color="DarkRed", Title="Item 1" },
                new Item { Color="DarkGray", Title="Item 2" },
                new Item { Color="Brown", Title="Item 3" },
                new Item { Color="DarkGreen", Title="Item 4" },
                new Item { Color="DarkOrange", Title="Item 5" },
                new Item { Color="DarkCyan", Title="Item 6" },
            };
            return result;
        }

        public ObservableCollection<Item> Items { get; set; }


        public List<VariableItem> GetVariableSizedItems()
        {
            var result = new List<VariableItem>()
            {
                new VariableItem { ColumnSpan=2,RowSpan=2,Color="DarkRed", Title="Item 1" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkGray", Title="Item 2" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="Brown", Title="Item 3" },
                new VariableItem { ColumnSpan=2,RowSpan=2,Color="DarkGreen", Title="Item 4" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkOrange", Title="Item 5" },
                new VariableItem { ColumnSpan=1,RowSpan=1,Color="DarkCyan", Title="Item 6" },
            };
            return result;
        }

        public ObservableCollection<VariableItem> VariableSizedItems { get; set; }


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
