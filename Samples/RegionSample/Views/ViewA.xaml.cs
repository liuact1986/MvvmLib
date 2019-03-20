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
    /// Logique d'interaction pour ViewA.xaml
    /// </summary>
    public partial class ViewA : UserControl, IActivatable
    {
        public ViewA()
        {
            InitializeComponent();
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Activate View A?", "Activate (code-behind)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }
}
