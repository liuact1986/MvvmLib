using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Activation guard for views and view models.
    /// </summary>
    public interface ICanActivate
    {
        /// <summary>
        /// Checks if can activate the view or view model.
        /// </summary>
        /// <param name="navigationContext">The navigation context</param>
        /// <param name="continuationCallback">The continuation callback</param>
        void CanActivate(NavigationContext navigationContext, Action<bool> continuationCallback);
    }
}
