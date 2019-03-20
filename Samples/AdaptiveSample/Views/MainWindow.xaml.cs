using AdaptiveSample.Views;
using MvvmLib.Navigation;
using System;
using System.Windows;

namespace AdaptiveSample.Views
{
    public partial class MainWindow : Window
    {
        IRegionManager regionManager;

        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();

            this.regionManager = regionManager;

            this.Activated += MainWindow_Activated;
        }

        private async  void MainWindow_Activated(object sender, EventArgs e)
        {
           await regionManager.GetContentRegion("Main").NavigateAsync(typeof(HomeView));
        }
    }
}
