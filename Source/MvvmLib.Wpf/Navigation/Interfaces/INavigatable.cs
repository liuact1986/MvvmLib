namespace MvvmLib.Navigation
{
    public interface INavigatable
    {
        void OnNavigatingFrom();
        void OnNavigatedTo(object parameter);
    }
}
