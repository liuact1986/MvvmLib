using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AdaptiveSample.Windows
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.MainFrame.Navigate(typeof(PageOne));
        }

        private void OnGoDataTemplateSelectorPage(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(WithDataTemplateSelectorPage));
        }

        private void OnGoResponsiveGridViewPage(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(GridViewDemoPage));
        }

        private void OnGoResponsiveVariableSizedGridViewPage(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(ResponsiveVariableSizedGridViewDemoPage));
        }

        private void OnGoBreakpointPage(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(WithBreakpointPage));
        }

        private void OnGoAdaptiveDemo(object sender, RoutedEventArgs e)
        {
            this.MainFrame.Navigate(typeof(PageOne));
        }

        private async void OnShowBusy(object sender, RoutedEventArgs e)
        {

            this.BusyIndicator.IsActive = true;

            await Task.Delay(2000);

            this.BusyIndicator.IsActive = false;

        }
    }
}
