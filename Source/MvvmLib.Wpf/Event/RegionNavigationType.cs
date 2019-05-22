namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation type.
    /// </summary>
    public enum RegionNavigationType
    {
        /// <summary>
        /// On navigate.
        /// </summary>
        New,
        /// <summary>
        /// On go back.
        /// </summary>
        Back,
        /// <summary>
        /// On go forward.
        /// </summary>
        Forward,
        /// <summary>
        /// On redirect.
        /// </summary>
        Redirect,
        /// <summary>
        /// On insert.
        /// </summary>
        Insert,
        /// <summary>
        /// On remove.
        /// </summary>
        Remove,
        /// <summary>
        /// On go root.
        /// </summary>
        Root
    }

}