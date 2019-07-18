using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewGViewModel : BindableBase, ICanActivate, ICanDeactivate
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewGViewModel()
        {
            Message = "Default ViewG Message";
        }

        public Task<bool> CanActivate(NavigationContext navigationContext)
        {
            var canActivate = MessageBox.Show("ACTIVATE ViewG [ViewModel]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(canActivate);
        }

        public Task<bool> CanDeactivate(NavigationContext navigationContext)
        {
            var canDeactivate = MessageBox.Show("DEACTIVATE ViewG [ViewModel]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(canDeactivate);
        }
    }
}
