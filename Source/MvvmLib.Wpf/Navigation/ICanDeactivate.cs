using System;
using System.Threading.Tasks;

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
        /// <param name="continuationCallback">The continuation callback</param>
        void CanDeactivate(Action<bool> continuationCallback);
    }
}
