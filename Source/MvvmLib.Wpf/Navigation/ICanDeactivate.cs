using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Deactivation guard for views and view models.
    /// </summary>
    public interface ICanDeactivate
    {
        /// <summary>
        /// Checks if can deactivate the view or view model.
        /// </summary>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="continuationCallback">The continuation callback</param>
        void CanDeactivate(NavigationContext navigationContext, Action<bool> continuationCallback);
    }
}
