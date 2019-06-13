using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class MultipleSubscribersSampleView : UserControl
    {
        public MultipleSubscribersSampleView()
        {
            InitializeComponent();
        }

        private void OnNewWindowClick(object sender, RoutedEventArgs e)
        {
            var window = new Window1();
            window.Show();
        }
    }
}
