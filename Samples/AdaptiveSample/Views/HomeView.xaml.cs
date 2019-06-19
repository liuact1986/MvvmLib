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

            this.Navigation = NavigationManager.GetNavigationSources("Main")[0];
        }

        private  void OnGoBreakpoint(object sender, RoutedEventArgs e)
        {
             Navigation.Navigate(typeof(Scenario1));
        }

        private  void OnGoDataContext(object sender, RoutedEventArgs e)
        {
             Navigation.Navigate(typeof(Scenario2));
        }

        private  void OnGoAsControl(object sender, RoutedEventArgs e)
        {
             Navigation.Navigate(typeof(Scenario3));
        }
    }
}
