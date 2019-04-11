namespace MvvmLib.Navigation
{
    /// <summary>
    /// The Navigation manager contract.
    /// </summary>
    public interface INavigationManager
    {
        /// <summary>
        /// Returns the default navigation service.
        /// </summary>
        /// <returns>The navigation service</returns>
        IFrameNavigationService GetDefault();

        /// <summary>
        /// Returns the named navigation service.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The navigation service</returns>
        IFrameNavigationService GetNamed(string name);
    }
}