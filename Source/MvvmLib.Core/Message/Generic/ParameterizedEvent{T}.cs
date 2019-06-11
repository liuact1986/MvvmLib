using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// Event with event args or parameter.
    /// </summary>
    public class ParameterizedEvent<TPayload> : IEvent
    {
        /// <summary>
        /// Payload : EventArgs class or string, ...
        /// </summary>
        private readonly List<Subscriber<TPayload>> subscribers;

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
        /// Creates the <see cref="ParameterizedEvent{TPayload}"/>.
        /// </summary>
        public ParameterizedEvent()
        {
            subscribers = new List<Subscriber<TPayload>>();
        }

        /// <summary>
        /// Checks if a subscriber is registered for the action.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>True if found</returns>
        public bool Contains(Action<TPayload> action)
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
        /// Checks if a subscriber is registered for the action.
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="keepAlive">Allows to keep the reference alive</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions<TPayload> Subscribe(Action<TPayload> action, bool keepAlive)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var subscriptionToken = new SubscriptionToken(Unsubscribe);

            var weakAction = new WeakDelegate(action, keepAlive);
            var subscriber = new Subscriber<TPayload>(subscriptionToken, synchronizationContext, weakAction);

            lock (subscribers)
            {
                subscribers.Add(subscriber);
            }

            var options = new SubscriberOptions<TPayload>(subscriber);
            return options;
        }

        /// <summary>
        /// Checks if a subscriber is registered for the action.
        /// </summary>
        /// <param name="action">The action</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions<TPayload> Subscribe(Action<TPayload> action)
        {
            return Subscribe(action, false);
        }

        /// <summary>
        /// Allows to subscribe to the event.
        /// </summary>
        /// <returns>The subscription options</returns>
        public bool Unsubscribe(SubscriptionToken token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

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

        /// <summary>
        /// Notifies all subscribers.
        /// </summary>
        /// <param name="payload">The payload</param>
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
                {
                    foreach (var subscriberToRemove in subscribersToRemove)
                        subscribers.Remove(subscriberToRemove);
                }
            }
        }
    }
}
