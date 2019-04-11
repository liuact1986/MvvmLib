namespace MvvmLib.Navigation
{
    public interface INavigatable
    {
        void OnNavigatingFrom();
        void OnNavigatingTo(object parameter);
        void OnNavigatedTo(object parameter);
    }
}
