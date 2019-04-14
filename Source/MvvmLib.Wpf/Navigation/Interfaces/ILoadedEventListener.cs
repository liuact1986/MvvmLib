using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to notify the view model when view is loaded.
    /// </summary>
    public interface ILoadedEventListener
    {
        /// <summary>
        /// Invoked when view is loaded.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="parameter">The parameter</param>
        void OnLoaded(FrameworkElement view, object parameter);
    }
}
