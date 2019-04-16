using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MvvmLib.Message
{

    public interface IEventAggregator
    {
        TEvent GetEvent<TEvent>() where TEvent : IEvent, new();
    }

    public class EventAggregator : IEventAggregator
    {
        private readonly SynchronizationContext DefaultSynchronizationContext = SynchronizationContext.Current;

        private readonly ConcurrentDictionary<Type, IEvent> events = new ConcurrentDictionary<Type, IEvent>();

        private SynchronizationContext synchronizationContext;
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext ?? DefaultSynchronizationContext; }
            set { synchronizationContext = value; }
        }

        public TEvent GetEvent<TEvent>() where TEvent : IEvent, new()
        {
            return (TEvent)events.GetOrAdd(typeof(TEvent), _ => new TEvent { SynchronizationContext = synchronizationContext });
        }
    }
}
