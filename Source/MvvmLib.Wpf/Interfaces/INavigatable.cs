namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to notify the view model and / or the view.
    /// </summary>
    public interface INavigatable
    {
        /// <summary>
        /// Invoked before leaving the view.
        /// </summary>
        void OnNavigatingFrom();

        /// <summary>
        /// Allows to preload data before the content of the region has changed. 
        /// </summary>
        /// <param name="parameter">The parameter</param>
        void OnNavigatingTo(object parameter);

        /// <summary>
        /// Invoked after the content of the region has changed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        void OnNavigatedTo(object parameter);
    }
}
