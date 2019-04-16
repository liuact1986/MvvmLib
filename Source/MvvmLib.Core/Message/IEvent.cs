using System.Threading;

namespace MvvmLib.Message
{
    public interface IEvent
    {
        SynchronizationContext SynchronizationContext { get; set; }
    }
}
