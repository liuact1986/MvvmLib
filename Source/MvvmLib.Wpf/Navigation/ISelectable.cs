using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Allows to select an existing view model.
    /// </summary>
    public interface ISelectable
    {
        /// <summary>
        /// Checks if the current view model is the target.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if the view model is the target</returns>
        bool IsTarget(Type sourceType, object parameter);
    }
}
