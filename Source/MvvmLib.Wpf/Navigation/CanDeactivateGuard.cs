using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// Invokes <see cref="IDeactivatable.CanDeactivateAsync" /> for views and view models and returns the result.
    /// </summary>
    public class CanDeactivateGuard
    {
        /// <summary>
        /// Invokes <see cref="IDeactivatable.CanDeactivateAsync" /> if the view implements <see cref="IDeactivatable"/>.
        /// </summary>
        /// <param name="currentView">The view</param>
        /// <returns>True if Can Deactivate</returns>
        public async Task<bool> CanDeactivateViewAsync(FrameworkElement currentView)
        {
            if (currentView is IDeactivatable p)
            {
                var canDeactivate = await p.CanDeactivateAsync();
                return canDeactivate;
            }
            return true;
        }

        /// <summary>
        /// Invokes <see cref="IDeactivatable.CanDeactivateAsync" /> if the view model implements <see cref="IDeactivatable"/>.
        /// </summary>
        /// <param name="currentContext">The current DataContext</param>
        /// <returns>True if Can Deactivate</returns>
        public async Task<bool> CanDeactivateContextAsync(object currentContext)
        {
            if (currentContext is IDeactivatable p)
            {
                var canDeactivate = await p.CanDeactivateAsync();
                return canDeactivate;
            }
            return true;
        }
    }

   

}
