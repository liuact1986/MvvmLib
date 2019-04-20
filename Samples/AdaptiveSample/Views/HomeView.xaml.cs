using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MvvmLib.Navigation;

namespace AdaptiveSample.Views
{
    /// <summary>
    /// Logique d'interaction pour HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        IRegionNavigationService regionNavigationService;

        public HomeView(IRegionNavigationService regionNavigationService)
        {
            this.regionNavigationService = regionNavigationService;

            InitializeComponent();
        }

        private async void OnGoBreakpoint(object sender, RoutedEventArgs e)
        {
            await regionNavigationService.GetContentRegion("Main").NavigateAsync(typeof(Scenario1));
        }

        private async void OnGoDataContext(object sender, RoutedEventArgs e)
        {
            await regionNavigationService.GetContentRegion("Main").NavigateAsync(typeof(Scenario2));
        }

        private async void OnGoAsControl(object sender, RoutedEventArgs e)
        {
            await regionNavigationService.GetContentRegion("Main").NavigateAsync(typeof(Scenario3));
        }
    }
}
