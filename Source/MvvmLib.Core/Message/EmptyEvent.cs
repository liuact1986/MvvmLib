using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// Event with no event args or parameter.
    /// </summary>
    public class EmptyEvent : IEvent
    {
        private readonly List<Subscriber> subscribers;

        private SynchronizationContext synchronizationContext;
        /// <summary>
        /// The synchronization context.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
            set { synchronizationContext = value; }
        }

        /// <summary>
        /// Creates the <see cref="EmptyEvent"/>
        /// </summary>
        public EmptyEvent()
        {
            subscribers = new List<Subscriber>();
        }

        /// <summary>
        /// Checks if a subscriber is registered for the action.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>True if registered</returns>
        public bool Contains(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action)); 

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.Action == action);
                return subscriber != null;
            }
        }

        /// <summary>
        /// Allows to subscribe to the event.
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="keepAlive">Allows to keep the reference alive</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions Subscribe(Action action, bool keepAlive)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action)); 

            var subscriptionToken = new SubscriptionToken(Unsubscribe);

            var weakAction = new WeakDelegate(action, keepAlive);
            var subscriber = new Subscriber(subscriptionToken, synchronizationContext, weakAction);

            lock (subscribers)
            {
                subscribers.Add(subscriber);
            }

            var options = new SubscriberOptions(subscriber);
            return options;
        }

        /// <summary>
        /// Allows to subscribe to the event.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions Subscribe(Action action)
        {
            return this.Subscribe(action, false);
        }

        /// <summary>
        /// Allows to unsubscribe with the subscription token.
        /// </summary>
        /// <param name="token">The subscription token</param>
        /// <returns>True if unsubscribed</returns>
        public bool Unsubscribe(SubscriptionToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token)); 

            lock (subscribers)
            {
                var subscriber = subscribers.FirstOrDefault(s => s.SubscriptionToken == token);
                if (subscriber != null)
                   return subscribers.Remove(subscriber);
            }
            return false;
        }

        /// <summary>
        /// Clears all subscribers.
        /// </summary>
        public void UnsubscribeAll()
        {
            lock (subscribers)
            {
                subscribers.Clear();
            }
        }

        /// <summary>
        /// Notifies all subscribers.
        /// </summary>
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
