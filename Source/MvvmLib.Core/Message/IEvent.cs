using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// The event contract.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The synchronization context.
        /// </summary>
        SynchronizationContext SynchronizationContext { get; set; }
    }
}
