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
    public partial class ViewE : UserControl, IActivatable, IDeactivatable
    {
        public ViewE()
        {
            InitializeComponent();
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Activate View E?", "Activate (code-behind)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Deactivate View E?", "Deactivate (code-behind)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }
}
