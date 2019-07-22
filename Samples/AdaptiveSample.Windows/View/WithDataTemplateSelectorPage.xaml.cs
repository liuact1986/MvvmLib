using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MvvmLib.Adaptive;
using MvvmLib.Commands;
using MvvmLib.Mvvm;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveSample.Windows
{
    public sealed partial class WithDataTemplateSelectorPage : Page
    {
        public WithDataTemplateSelectorPage()
        {
            this.InitializeComponent();

            var items = GetItems();
            Items = new ObservableCollection<VariableItem>(items);
        }

        public List<VariableItem> GetItems()
        {
            var result = new List<VariableItem>()
            {
                new VariableItem { ItemType=DataItemType.Square, ColumnSpan=2,RowSpan=2,Color="DarkRed", Title="Item 1" },
                new VariableItem { ItemType=DataItemType.Border, ColumnSpan=1,RowSpan=1,Color="DarkGray", Title="Item 2" },
                new VariableItem { ItemType=DataItemType.Square, ColumnSpan=1,RowSpan=1,Color="Brown", Title="Item 3" },
                new VariableItem { ItemType=DataItemType.Border, ColumnSpan=2,RowSpan=2,Color="DarkGreen", Title="Item 4" },
                new VariableItem { ItemType=DataItemType.Square, ColumnSpan=1,RowSpan=1,Color="DarkOrange", Title="Item 5" },
                new VariableItem { ItemType=DataItemType.Border, ColumnSpan=1,RowSpan=1,Color="DarkCyan", Title="Item 6" },
            };
            return result;
        }

        public ObservableCollection<VariableItem> Items { get; set; }

        public ICommand Command
        {
            get
            {
                return new DelegateCommand<object>(async (r) =>
                {
                    var dialog = new MessageDialog("Clicked on : " + ((VariableItem)r).Title);
                    await dialog.ShowAsync();
                });
            }
        }
    }

    public class MyDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SquareTemplate { get; set; }
        public DataTemplate BorderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var variableItem = item as VariableItem;
            if (variableItem != null)
            {
                if (variableItem.ItemType == DataItemType.Square)
                {
                    return SquareTemplate;
                }
                else
                {
                    return BorderTemplate;
                }
            }

            return base.SelectTemplateCore(item, container);
        }
    }

}
