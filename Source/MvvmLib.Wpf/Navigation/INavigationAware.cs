namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to notify the view model and / or the view.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Invoked before leaving the view.
        /// </summary>
        /// <param name="navigationContext">The navigation context</param>
        void OnNavigatingFrom(NavigationContext navigationContext);

        /// <summary>
        /// Allows to preload data before the content of the region has changed. 
        /// </summary>
        /// <param name="navigationContext">The navigation context</param>
        void OnNavigatingTo(NavigationContext navigationContext);

        /// <summary>
        /// Invoked after the content of the region has changed.
        /// </summary>
        /// <param name="navigationContext">The navigation context</param>
        void OnNavigatedTo(NavigationContext navigationContext);
    }
}
