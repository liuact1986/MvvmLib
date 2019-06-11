using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// Contract for <see cref="EmptyEvent"/> and <see cref="ParameterizedEvent{TPayload}"/>.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// The synchronization context.
        /// </summary>
        SynchronizationContext SynchronizationContext { get; set; }
    }
}
