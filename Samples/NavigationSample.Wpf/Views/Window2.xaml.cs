using MvvmLib.Navigation;
using NavigationSample.Wpf.ViewModels;
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
using System.Windows.Shapes;

namespace NavigationSample.Wpf.Views
{
    /// <summary>
    /// Logique d'interaction pour Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();

            var from = NavigationManager.GetOrCreateSharedSource<MyItemDetailsViewModel>();

            var to = new SharedSource<MyItemDetailsViewModel>();
            to.Sync(from);
            TabControl1.ItemsSource = to.Items;
            TabControl1.SelectedItem = to.SelectedItem;
        }
    }
}
