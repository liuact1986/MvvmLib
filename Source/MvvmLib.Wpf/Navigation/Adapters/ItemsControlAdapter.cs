using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class ItemsControlAdapter : ItemsRegionAdapterBase<ItemsControl>
    {
        public override void OnInsert(ItemsControl control, object view, int index)
        {
            if (index >= 0 && index <= control.Items.Count)
            {
                control.Items.Insert(index, view);
            }
        }

        public override void OnRemoveAt(ItemsControl control, int index)
        {
            if (index >= 0 && index < control.Items.Count)
            {
                control.Items.RemoveAt(index);
            }
        }

        public override void OnClear(ItemsControl control)
        {
            control.Items.Clear();
        }
    }
}