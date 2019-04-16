using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MvvmLib.Message
{
    public class ParameterizedEvent<TPayload> : IEvent
    {
        /// <summary>
        /// Payload : EventArgs class or string, ...
        /// </summary>
        private readonly List<Subscriber<TPayload>> subscribers = new List<Subscriber<TPayload>>();

        private SynchronizationContext synchronizationContext;
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
            set { synchronizationContext = value; }
        }

        public bool Contains(Action<TPayload> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.Action == action);
                return subscriber != null;
            }
        }

        public SubscriberOptions<TPayload> Subscribe(Action<TPayload> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            var subscriptionToken = new SubscriptionToken(Unsubscribe);

            var weakAction = new WeakDelegate(action);
            var subscriber = new Subscriber<TPayload>(subscriptionToken, synchronizationContext, weakAction);

            lock (subscribers)
            {
                subscribers.Add(subscriber);
            }

            var options = new SubscriberOptions<TPayload>(subscriber);
            return options;
        }

        public bool Unsubscribe(SubscriptionToken token)
        {
            if (token == null) { throw new ArgumentNullException(nameof(token)); }

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.SubscriptionToken == token);
                if (subscriber != null)
                {
                    subscribers.Remove(subscriber);
                    return true;
                }
            }
            return false;
        }

        public void Publish(TPayload payload)
        {
            lock (subscribers)
            {
                var subscribersToRemove = new List<Subscriber<TPayload>>();
                foreach (var subscriber in subscribers)
                {
                    var action = subscriber.Action;
                    var filter = subscriber.Filter;
                    if (action == null || filter == null)
                        subscribersToRemove.Add(subscriber);
                    else
                    {
                        if (filter.Invoke(payload))
                            subscriber.Invoke(action, payload);
                    }
                }

                if (subscribersToRemove.Count > 0)
                    foreach (var subscriberToRemove in subscribersToRemove)
                        subscribers.Remove(subscriberToRemove);
            }
        }
    }
}
