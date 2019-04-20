using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The <see cref="ItemsControl"/> adapter.
    /// </summary>
    public class ItemsControlAdapter : ItemsRegionAdapterBase<ItemsControl>
    {
        /// <summary>
        /// Invoked on insert new content.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="content">The content</param>
        /// <param name="index">The index</param>
        public override void OnInsert(ItemsControl control, object content, int index)
        {
            if (index >= 0 && index <= control.Items.Count)
                control.Items.Insert(index, content);
        }

        /// <summary>
        /// Invoked on remove element.
        /// </summary>
        /// <param name="control">The control</param>
        /// <param name="index">The index</param>
        public override void OnRemoveAt(ItemsControl control, int index)
        {
            if (index >= 0 && index < control.Items.Count)
                control.Items.RemoveAt(index);
        }

        /// <summary>
        /// Invoked on clear all elements.
        /// </summary>
        /// <param name="control">The control</param>
        public override void OnClear(ItemsControl control)
        {
            control.Items.Clear();
        }
    }
}