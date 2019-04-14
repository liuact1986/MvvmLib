using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Activation guard for views and view models.
    /// </summary>
    public interface IActivatable
    {
        /// <summary>
        /// Checks if can activate the view or view model.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True or false</returns>
        Task<bool> CanActivateAsync(object parameter);
    }

    public interface IRegionKnowledge
    {
        void GetRegion(IRegion region);
    }

    public interface IRegionKnowledge<T> where T : IRegion
    {
        void GetRegion(T region);
    }
}
