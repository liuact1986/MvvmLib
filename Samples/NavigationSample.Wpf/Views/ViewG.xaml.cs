using MvvmLib.Navigation;
using System;
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

        public void CanActivate(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var canActivate = MessageBox.Show("ACTIVATE ViewG [Code-Behind]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            continuationCallback(canActivate);
        }

        public void CanDeactivate(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            var canDeactivate = MessageBox.Show("DEACTIVATE ViewG [Code-Behind]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            continuationCallback(canDeactivate);
        }
    }
}
