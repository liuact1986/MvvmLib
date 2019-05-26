using System.Windows.Input;

namespace NavigationSample.Wpf.ViewModels
{
    public interface IDetailViewModel
    {
        string Message { get; set; }

        ICommand CloseCommand { get; }
    }
}
