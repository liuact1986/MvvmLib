using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmLib.Message
{
    /// <summary>
    /// Subscriber with parameter.
    /// </summary>
    /// <typeparam name="TPayload"></typeparam>
    public sealed class Subscriber<TPayload> : SubscriberBase
    {
        internal WeakDelegate weakAction;
        /// <summary>
        /// The action to call.
        /// </summary>
        public Action<TPayload> Action
        {
            get { return (Action<TPayload>)weakAction.Target; }
        }

        internal WeakDelegate weakFilter;
        /// <summary>
        /// The filter.
        /// </summary>
        public Func<TPayload, bool> Filter
        {
            get { return (Func<TPayload, bool>)weakFilter.Target; }
        }
        /// <summary>
        /// Creates the <see cref="Subscriber{TPayload}"/>.
        /// </summary>
        /// <param name="subscriptionToken">The subscription token</param>
        /// <param name="synchronizationContext">The synchronization context</param>
        /// <param name="weakAction">The action</param>
        public Subscriber(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext, WeakDelegate weakAction)
             : base(subscriptionToken, synchronizationContext)
        {
            if (weakAction == null)
                throw new ArgumentNullException(nameof(weakAction));

            this.weakAction = weakAction;

            var defaultFilter = new Func<TPayload, bool>(_ => true);
            this.weakFilter = new WeakDelegate(defaultFilter, false);
        }

        /// <summary>
        /// Invokes the action.
        /// </summary>
        /// <param name="action">The action</param>
        /// <param name="payload">The payload</param>
        public void Invoke(Action<TPayload> action, TPayload payload)
        {
            switch (ExecutionStrategy)
            {
                case ExecutionStrategyType.PublisherThread:
                    action(payload);
                    break;
                case ExecutionStrategyType.UIThread:
                    if (this.SynchronizationContext == null)
                        throw new InvalidOperationException($"Cannot invoke the action with \"{ExecutionStrategy}\". The Synchronization context is null"); 

                    this.SynchronizationContext.Post(_ => action(payload), payload);
                    break;
                case ExecutionStrategyType.BackgroundThread:
                    Task.Run(() => action(payload));
                    break;
            }
        }
    }
}
