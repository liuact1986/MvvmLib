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
        /// <param name="parameter">The parameter</param>
        /// <param name="continuationCallback">The continuation callback</param>
        void CanActivate(object parameter, Action<bool> continuationCallback);
    }
}
