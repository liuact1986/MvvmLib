using MvvmLib.Navigation;
using System.Windows;
using System.Windows.Controls;

namespace RegionSample.Adapters
{
    public class StackPanelRegionAdapter : ItemsRegionAdapterBase<StackPanel>
    {
        public override void OnClear(StackPanel control)
        {
            control.Children.Clear();
        }

        public override void OnInsert(StackPanel control, object view, int index)
        {
            if (index >= 0 && index <= control.Children.Count)
            {
                control.Children.Insert(index, (UIElement)view);
            }
        }

        public override void OnRemoveAt(StackPanel control, int index)
        {
            if (index >= 0 && index < control.Children.Count)
            {
                control.Children.RemoveAt(index);
            }
        }
    }
}
