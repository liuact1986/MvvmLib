using System;
using System.Windows;
using System.Windows.Controls;
using MvvmLib.Adaptive;
using MvvmLib.Navigation;

namespace AdaptiveSample.Views
{
    public partial class Scenario1 : UserControl
    {

        public Scenario1()
        {
            InitializeComponent();

            this.Navigation = NavigationManager.GetNavigationSources("Main")[0];

            var listener = new BreakpointListener();

            listener.AddBreakpoint(0);
            listener.AddBreakpoint(320);
            listener.AddBreakpoint(960);
            listener.AddBreakpoint(640);
            listener.AddBreakpoint(1280);
            listener.AddBreakpoint(1600);

            listener.BreakpointChanged += OnChanged;
        }

        public NavigationSource Navigation { get; }

        private void OnChanged(object sender, BreakpointChangedEventArgs e)
        {
            List.Items.Add($"Active breakpoint: {e.Width}px");
        }

        private async void OnGoBack(object sender, RoutedEventArgs e)
        {
            await Navigation.GoBackAsync();
        }
    }
}
