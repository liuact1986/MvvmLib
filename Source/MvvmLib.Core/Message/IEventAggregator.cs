namespace MvvmLib.Message
{
    /// <summary>
    /// Allows to exchange message between subscribers and publishers.
    /// </summary>
    public interface IEventAggregator
    {
        /// <summary>
        /// Gets or creates the event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <returns>The event class</returns>
        TEvent GetEvent<TEvent>() where TEvent : IEvent, new();
    }
}
