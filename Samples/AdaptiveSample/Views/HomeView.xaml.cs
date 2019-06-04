using MvvmLib.Navigation;
using System.Windows;
using System.Windows.Controls;

namespace AdaptiveSample.Views
{
    public partial class HomeView : UserControl
    {
        public NavigationSource Navigation { get; }

        public HomeView()
        {
            InitializeComponent();

            this.Navigation = NavigationManager.GetNavigationSource("Main");
        }

        private async void OnGoBreakpoint(object sender, RoutedEventArgs e)
        {
            await Navigation.NavigateAsync(typeof(Scenario1));
        }

        private async void OnGoDataContext(object sender, RoutedEventArgs e)
        {
            await Navigation.NavigateAsync(typeof(Scenario2));
        }

        private async void OnGoAsControl(object sender, RoutedEventArgs e)
        {
            await Navigation.NavigateAsync(typeof(Scenario3));
        }
    }
}
