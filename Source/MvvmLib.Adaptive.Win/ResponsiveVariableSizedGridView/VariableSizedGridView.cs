using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Adaptive
{
    public sealed class VariableSizedGridView : GridView
    {
        public VariableSizedGridView()
        {
            this.DefaultStyleKey = typeof(VariableSizedGridView);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var dataItem = item as VariableSizedGridViewItem;
            if (dataItem == null) throw new ArgumentException("Items for VariableSizedGridView have to inherit from VariableSizedGridViewItem");

            var gridViewItem = element as GridViewItem;
            if (gridViewItem != null)
            {
                if (dataItem.ColumnSpan > 0) VariableSizedWrapGrid.SetColumnSpan(gridViewItem, dataItem.ColumnSpan);
                if (dataItem.RowSpan > 0) VariableSizedWrapGrid.SetRowSpan(gridViewItem, dataItem.RowSpan);
            }

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
