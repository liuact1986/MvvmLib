﻿namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to notify the view model when view is loaded.
    /// </summary>
    public interface IIsLoaded
    {
        /// <summary>
        /// Invoked when view is loaded.
        /// </summary>
        void OnLoaded();
    }
}
