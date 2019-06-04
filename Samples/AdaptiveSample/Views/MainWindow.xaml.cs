using AdaptiveSample.Views;
using MvvmLib.Navigation;
using System;
using System.Windows;

namespace AdaptiveSample.Views
{
    public partial class MainWindow : Window
    {
        public NavigationSource Navigation { get; }

        public MainWindow()
        {
            InitializeComponent();

            this.Navigation = NavigationManager.GetNavigationSource("Main");
            NavigateToHome();
        }

        private async  void NavigateToHome()
        {
           await Navigation.NavigateAsync(typeof(HomeView));
        }
    }
}
