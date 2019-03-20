namespace MvvmLib.Message
{
    public interface IMessenger
    {
        TEventType GetEvent<TEventType>() where TEventType : EventBase, new();
        bool IsEventRegistered<TEventType>();
        bool RemoveEvent<TEventType>();
    }
}