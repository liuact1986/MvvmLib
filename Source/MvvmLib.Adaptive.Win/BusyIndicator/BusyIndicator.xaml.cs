using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MvvmLib.Adaptive
{
    public sealed partial class BusyIndicator : UserControl
    {
        public static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BusyIndicator), new PropertyMetadata("Loading ..."));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, OnActiveChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BusyIndicator;
            control.Visibility = (bool)e.NewValue == false ? Visibility.Collapsed : Visibility.Visible;
        }

        public BusyIndicator()
        {
            this.InitializeComponent();
        }

    }
}
