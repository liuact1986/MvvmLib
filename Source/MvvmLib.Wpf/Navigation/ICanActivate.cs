using System;
using System.Threading.Tasks;

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
        Task<bool> CanActivate(NavigationContext navigationContext);
    }
}
