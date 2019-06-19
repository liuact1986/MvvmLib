using MvvmLib.Navigation;
using NavigationSample.Wpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class HistorySampleView : UserControl
    {
        public HistorySampleView()
        {
            InitializeComponent();
        }

        private async void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            var frameworkElement = sender as FrameworkElement;
            var item = frameworkElement.DataContext as SourceMenuItem;

            var vm = this.DataContext as HistorySampleViewModel;
            vm.Navigation.MoveTo(item.Index);
        }
    }
}
