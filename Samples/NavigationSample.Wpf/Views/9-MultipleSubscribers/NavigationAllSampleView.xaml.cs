using NavigationSample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class NavigationAllSampleView : UserControl
    {
        public NavigationAllSampleView()
        {
            InitializeComponent();
        }

        private void OnNewWindowClick(object sender, RoutedEventArgs e)
        {
            var window = new Window1();
            window.Show();
        }

        private void OnSyncClick(object sender, RoutedEventArgs e)
        {
            var window = new Window3();
            window.Show();
        }
    }

    public class DetailsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ViewA || item is ViewB || item is ViewD)
            {
                return EditTemplate;
            }
            if (item is ViewAViewModel || item is ViewBViewModel || item is ViewDViewModel)
            {
                return ViewModelEditTemplate;
            }

            return base.SelectTemplate(item, container);
        }

        public DataTemplate EditTemplate { get; set; }
        public DataTemplate ViewModelEditTemplate { get; set; }
    }
}
