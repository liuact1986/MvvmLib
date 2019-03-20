using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Allows to notify view model on navigate.
    /// </summary>
    public interface INavigatable
    {
        /// <summary>
        /// Invoked after page navigated. 
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <param name="navigationMode">The navigation mode</param>
        void OnNavigatedTo(object parameter, NavigationMode navigationMode);

        /// <summary>
        /// Invoked before page navigated. 
        /// </summary>
        /// <param name="isSuspending">Indicates if the application is suspending</param>
        void OnNavigatingFrom(bool isSuspending);
    }
}