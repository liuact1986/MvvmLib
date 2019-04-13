using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Deactivation guard for views and view models.
    /// </summary>
    public interface IDeactivatable
    {
        /// <summary>
        /// Checks if can deactivate the view or view model.
        /// </summary>
        /// <returns>True or false</returns>
        Task<bool> CanDeactivateAsync();
    }
}
