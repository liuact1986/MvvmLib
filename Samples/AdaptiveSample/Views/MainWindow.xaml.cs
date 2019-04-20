using AdaptiveSample.Views;
using MvvmLib.Navigation;
using System;
using System.Windows;

namespace AdaptiveSample.Views
{
    public partial class MainWindow : Window
    {
        IRegionNavigationService regionNavigationService;

        public MainWindow(IRegionNavigationService regionNavigationService)
        {
            InitializeComponent();

            this.regionNavigationService = regionNavigationService;

            this.Activated += MainWindow_Activated;
        }

        private async  void MainWindow_Activated(object sender, EventArgs e)
        {
           await regionNavigationService.GetContentRegion("Main").NavigateAsync(typeof(HomeView));
        }
    }
}
