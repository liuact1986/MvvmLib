namespace MvvmLib.Logger
{
    public interface ILogger
    {
        void Log(string message, Category category, Priority priority);
    }
}