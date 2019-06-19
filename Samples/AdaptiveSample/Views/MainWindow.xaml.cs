using AdaptiveSample.Views;
using MvvmLib.Navigation;
using System;
using System.Windows;

namespace AdaptiveSample.Views
{
    public partial class MainWindow : Window
    {
        public NavigationSource Navigation { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            this.Navigation = NavigationManager.GetNavigationSources("Main")[0];
            NavigateToHome();
        }


        private void NavigateToHome()
        {
           Navigation.Navigate(typeof(HomeView));
        }
    }
}
