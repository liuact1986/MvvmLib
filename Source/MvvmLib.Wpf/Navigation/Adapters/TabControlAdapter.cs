using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The <see cref="TabControl"/> adapter.
    /// </summary>
    public class TabControlAdapter : ItemsRegionAdapterBase<TabControl>
    {
        private TabItem CreateTabItem(object content)
        {
            var tabItem = new TabItem
            {
                Content = content,
                DataContext = content != null && content is FrameworkElement element ? element.DataContext : null
            };
            return tabItem;
        }
        /// <summary>
        /// Invoked on insert new content.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="content">The content</param>
        /// <param name="index">The index</param>
        public override void OnInsert(TabControl control, object content, int index)
        {
            if (index >= 0 && index <= control.Items.Count)
            {
                var tabItem = CreateTabItem(content);
                control.Items.Insert(index, tabItem);
                //tabItem.Focus();
                control.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Invoked on remove element.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="index">The index</param>
        public override void OnRemoveAt(TabControl control, int index)
        {
            if (index >= 0 && index < control.Items.Count)
                control.Items.RemoveAt(index);
        }

        /// <summary>
        /// Invoked on clear all elements.
        /// </summary>
        /// <param name="control">The control</param>
        public override void OnClear(TabControl control)
        {
            control.Items.Clear();
        }

    }
}