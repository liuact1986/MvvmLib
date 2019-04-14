using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to select a view model.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Checks if the current view model is target.
        /// </summary>
        /// <param name="viewType">The view type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if the view model is the target</returns>
        bool IsTarget(Type viewType, object parameter);
    }
}