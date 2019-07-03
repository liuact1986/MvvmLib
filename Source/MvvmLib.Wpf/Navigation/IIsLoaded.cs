namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to notify the view model when the view is loaded. Only available with <see cref="ViewModelLocator.ResolveViewModelProperty"/>.
    /// </summary>
    public interface IIsLoaded
    {
        /// <summary>
        /// Invoked when the view is loaded.
        /// </summary>
        void OnLoaded();
    }
}
