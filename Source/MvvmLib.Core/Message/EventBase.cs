using System;
using System.Collections.Generic;
using System.Threading;

namespace MvvmLib.Message
{
    public class EventBase
    {
        protected List<EventSubscription> subscriptions;

        public int SubcribersCount => subscriptions.Count;

        public bool UseCurrentSynchronizationContext { get; set; }

        private SynchronizationContext syncContext = SynchronizationContext.Current;
        protected SynchronizationContext SynchronizationContext => syncContext ?? new SynchronizationContext();

        public EventBase()
        {
            this.subscriptions = new List<EventSubscription>();
            this.UseCurrentSynchronizationContext = false;
        }

        protected EventSubscription RegisterSubscriber(Delegate subscriber, Delegate filter = null)
        {
            var subscription = new EventSubscription(subscriber, filter, (t) => this.subscriptions.Remove(t));
            this.subscriptions.Add(subscription);
            return subscription;
        }

        public void UnsubscribeAll()
        {
            foreach (var subscription in this.subscriptions)
            {
                subscription.Kill();
            }
            this.subscriptions.Clear();
        }

        public void Clean()
        {
            var toRemove = new List<EventSubscription>();
            foreach (var subscription in this.subscriptions)
            {
                if (!subscription.weakDelegate.IsAlive)
                {
                    toRemove.Add(subscription);
                }
            }
            foreach (var subscriber in toRemove)
            {
                this.subscriptions.Remove(subscriber);
            }
        }
    }

}

