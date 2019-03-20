using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Allows to cancel navigation from view model.
    /// </summary>
    public interface IDeactivatable
    {
        /// <summary>
        /// Checks if the navigation can continue.
        /// </summary>
        /// <returns>True to continue navigation</returns>
        Task<bool> CanDeactivateAsync();
    }
}