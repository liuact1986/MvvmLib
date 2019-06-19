using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System;
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

        public void CanActivate(object parameter, Action<bool> continuationCallback)
        {
            var canActivate = MessageBox.Show("ACTIVATE ViewG [ViewModel]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            continuationCallback(canActivate);
        }

        public void CanDeactivate(Action<bool> continuationCallback)
        {
            var canDeactivate = MessageBox.Show("DEACTIVATE ViewG [ViewModel]?", "Question", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            continuationCallback(canDeactivate);
        }
    }
}
