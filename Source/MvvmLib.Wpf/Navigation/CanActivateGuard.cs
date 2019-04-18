using System.Threading.Tasks;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Invokes <see cref="IActivatable.CanActivateAsync" /> for views and view models and returns the result.
    /// </summary>
    public class CanActivateGuard
    {
        /// <summary>
        ///  Invokes <see cref="IActivatable.CanActivateAsync" /> for the view.
        /// </summary>
        /// <param name="view">The view</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if Can Activate</returns>
        public async Task<bool> CanActivateViewAsync(FrameworkElement view, object parameter)
        {
            if (view is IActivatable p)
            {
                var canActivate = await p.CanActivateAsync(parameter);
                return canActivate;
            }
            return true;
        }


        /// <summary>
        /// Invokes <see cref="IActivatable.CanActivateAsync" /> for the view model.
        /// </summary>
        /// <param name="context">The DataContext</param>
        /// <param name="parameter">The paarmeter</param>
        /// <returns>True if Can Activate</returns>
        public async Task<bool> CanActivateContextAsync(object context, object parameter)
        {
            if (context is IActivatable p)
            {
                var canActivate = await p.CanActivateAsync(parameter);
                return canActivate;
            }
            return true;
        }
    }

}
