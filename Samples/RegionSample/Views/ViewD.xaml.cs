using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MvvmLib.Navigation;

namespace RegionSample.Views
{
    public partial class ViewD : UserControl, IActivatable, IDeactivatable, INavigatable
    {
        public ViewD()
        {
            InitializeComponent();
        }

        public Task<bool> CanActivateAsync(object parameter)
        {
            var result = MessageBox.Show("Activate View D?", "Activate (code-behind)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public Task<bool> CanDeactivateAsync()
        {
            var result = MessageBox.Show("Deactivate View D?", "Deactivate (code-behind)", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
            return Task.FromResult(result);
        }

        public void OnNavigatedTo(object parameter)
        {

        }

        public void OnNavigatingFrom()
        {
            MessageBox.Show("OnNavigatingFrom (code-behind)", "ViewD");
        }
    }
}
