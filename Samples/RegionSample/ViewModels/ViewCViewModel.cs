using System.Threading.Tasks;
using System.Windows;
using MvvmLib.Mvvm;
using MvvmLib.Navigation;

namespace RegionSample.ViewModels
{
    public class ViewCViewModel: BindableBase, IActivatable, IDeactivatable
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ViewCViewModel()
        {
            Message = "DataContext";
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Activate View C?", "Activate (VIEWMODEL)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult<bool>(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Deactivate View C?", "Deactivate", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }
    }
}
