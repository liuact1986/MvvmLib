using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MvvmLib.Message
{
    public class EmptyEvent : IEvent
    {
        private readonly List<Subscriber> subscribers = new List<Subscriber>();

        private SynchronizationContext synchronizationContext;
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
            set { synchronizationContext = value; }
        }

        public bool Contains(Action action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.Action == action);
                return subscriber != null;
            }
        }

        public SubscriberOptions Subscribe(Action action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            var subscriptionToken = new SubscriptionToken(Unsubscribe);

            var weakAction = new WeakDelegate(action);
            var subscriber = new Subscriber(subscriptionToken, synchronizationContext, weakAction);

            lock (subscribers)
            {
                subscribers.Add(subscriber);
            }

            var options = new SubscriberOptions(subscriber);
            return options;
        }

        public bool Unsubscribe(SubscriptionToken token)
        {
            if (token == null) { throw new ArgumentNullException(nameof(token)); }

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.SubscriptionToken == token);
                if (subscriber != null)
                   return subscribers.Remove(subscriber);
            }
            return false;
        }

        public void Publish()
        {
            lock (subscribers)
            {
                var subscribersToRemove = new List<Subscriber>();
                foreach (var subscriber in subscribers)
                {
                    var action = subscriber.Action;
                    if (action == null)
                        subscribersToRemove.Add(subscriber);
                    else
                        subscriber.Invoke(action);
                }

                if (subscribersToRemove.Count > 0)
                    foreach (var subscriberToRemove in subscribersToRemove)
                        subscribers.Remove(subscriberToRemove);
            }
        }
    }
}
