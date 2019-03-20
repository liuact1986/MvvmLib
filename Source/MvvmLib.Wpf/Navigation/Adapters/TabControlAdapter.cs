using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class TabControlAdapter : ItemsRegionAdapterBase<TabControl>
    {
        private TabItem CreateTabItem(object view)
        {
            return new TabItem
            {
                Content = view,
                DataContext = ((FrameworkElement)view).DataContext
            };
        }

        public override void OnInsert(TabControl control, object view, int index)
        {
            if (index >= 0 && index <= control.Items.Count)
            {
                var tabItem = CreateTabItem(view);
                control.Items.Insert(index, tabItem);
                //tabItem.Focus();
                control.SelectedIndex = index;
            }
        }

        public override void OnRemoveAt(TabControl control, int index)
        {
            if (index >= 0 && index < control.Items.Count)
            {
                control.Items.RemoveAt(index);
            }
        }

        public override void OnClear(TabControl control)
        {
            control.Items.Clear();
        }

    }
}