using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{
    public class DetailViewModel : BindableBase, INavigatable, IActivatable, IDeactivatable
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { SetProperty(ref count, value); }
        }

        public DetailViewModel()
        {
            message = "DetailViewModel message";
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Activate Detail View Model?", "Activate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult<bool>(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Deactivate Detail View Model?", "Deactivate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public void OnNavigatingTo(object parameter)
        {
            Count++;
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {
            
        }
    }
}
