using NavigationSample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class NavigationSourceSampleView : UserControl
    {
        public NavigationSourceSampleView()
        {
            InitializeComponent();
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var item = frameworkElement.DataContext as SourceMenuItem;

            var vm = this.DataContext as NavigationSourceSampleViewModel;
            vm.Navigation.MoveTo(item.Index);
        }
    }
}
