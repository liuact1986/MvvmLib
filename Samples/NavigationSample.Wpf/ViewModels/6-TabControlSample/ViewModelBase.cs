using MvvmLib.Mvvm;
using MvvmLib.Navigation;
using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        private string message;
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public SharedSource<IDetailViewModel> DetailsSource { get; }

        public ICommand CloseCommand { get; }

        public ViewModelBase()
        {
            DetailsSource = NavigationManager.GetOrCreateSharedSource<IDetailViewModel>();

            CloseCommand = new RelayCommand<IDetailViewModel>(async (item) =>
            {
                await DetailsSource.Items.RemoveAsync(item);
            });
        }
    }
}
