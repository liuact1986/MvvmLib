using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    public interface IDeactivatable
    {
        Task<bool> CanDeactivateAsync();
    }
}
