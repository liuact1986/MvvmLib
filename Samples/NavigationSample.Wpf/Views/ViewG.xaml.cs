using MvvmLib.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NavigationSample.Wpf.Views
{
    public partial class ViewG : UserControl, ICanActivate, ICanDeactivate
    {
        public ViewG()
        {
            InitializeComponent();
        }


        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            var canActivate = MessageBox.Show("ACTIVATE ViewG [code-behind]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(canActivate);
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            var canDeactivate = MessageBox.Show("DEACTIVATE ViewG [code-behind]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(canDeactivate);
        }
    }
}
