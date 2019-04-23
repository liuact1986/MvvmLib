using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// Allows to exchange message between subscribers and publishers.
    /// </summary>
    public class EventAggregator : IEventAggregator
    {
        private readonly SynchronizationContext DefaultSynchronizationContext = SynchronizationContext.Current;
        private readonly ConcurrentDictionary<Type, IEvent> events = new ConcurrentDictionary<Type, IEvent>();

        private SynchronizationContext synchronizationContext;
        /// <summary>
        /// The current synchronization context.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext ?? DefaultSynchronizationContext; }
            set { synchronizationContext = value; }
        }

        /// <summary>
        /// Gets or create the event.
        /// </summary>
        /// <typeparam name="TEvent">The event type</typeparam>
        /// <returns>The event class</returns>
        public TEvent GetEvent<TEvent>() where TEvent : IEvent, new()
        {
            return (TEvent)events.GetOrAdd(typeof(TEvent), _ => new TEvent { SynchronizationContext = synchronizationContext });
        }
    }
}
