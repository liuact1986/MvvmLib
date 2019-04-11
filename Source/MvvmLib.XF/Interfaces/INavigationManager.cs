namespace MvvmLib.Navigation
{
    public interface INavigationManager
    {
        INavigationService GetDefault();
        INavigationService GetNamed(string name);
    }
}