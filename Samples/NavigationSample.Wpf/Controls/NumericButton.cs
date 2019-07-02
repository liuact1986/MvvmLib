using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Controls
{
    public class NumericButton : RadioButton
    {
        static NumericButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericButton), new FrameworkPropertyMetadata(typeof(NumericButton)));
        }

        public bool IsEllipsisButton { get; internal set; }

        public int PageIndex { get; internal set; }
    }
}
