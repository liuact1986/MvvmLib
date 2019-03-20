using System.Threading.Tasks;

namespace MvvmLib.Navigation
{
    public interface IActivatable
    {
        Task<bool> CanActivateAsync(object parameter);
    }
}
