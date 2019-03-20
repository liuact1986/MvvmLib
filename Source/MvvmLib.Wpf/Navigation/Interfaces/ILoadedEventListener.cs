using System.Windows;

namespace MvvmLib.Navigation
{
    public interface ILoadedEventListener
    {
        void OnLoaded(object parameter);
    }
}
