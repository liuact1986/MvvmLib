using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveSample.Windows
{
    public sealed partial class ResponsiveVariableSizedGridViewDemoPage : Page
    {
        public ResponsiveVariableSizedGridViewDemoPage()
        {
            this.InitializeComponent();
            this.SizeChanged += OnSizeChanged;

            var items = GetItems();
            Items = new ObservableCollection<VariableItem>(items);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 960)
            {
                GridView.Columns = 6;
            }
            else if (e.NewSize.Width > 600)
            {
                GridView.Columns = 4;
            }
            else
            {
                GridView.Columns = 2;
            }
        }

        public List<VariableItem> GetItems()
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
        public ObservableCollection<VariableItem> Items { get; set; }

    }

}
