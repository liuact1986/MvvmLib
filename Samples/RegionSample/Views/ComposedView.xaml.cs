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

namespace RegionSample.Views
{
    /// <summary>
    /// Logique d'interaction pour ComposedView.xaml
    /// </summary>
    public partial class ComposedView : UserControl
    {
        IRegionManager regionManager;

        public ComposedView(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await regionManager.GetContentRegion("RegionRight").NavigateAsync(typeof(ViewD));
        }
    }
}
