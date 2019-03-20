namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to view model to be notifed when the view is loaded.
    /// </summary>
    public interface ILoadedEventListener
    {
        /// <summary>
        /// Invoked on loaded.
        /// </summary>
        void OnLoaded();
    }
}