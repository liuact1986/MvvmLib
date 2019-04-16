using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmLib.Message
{
    public sealed class Subscriber<TPayload> : SubscriberBase
    {
        internal WeakDelegate weakAction;
        public Action<TPayload> Action
        {
            get { return (Action<TPayload>)weakAction.Target; }
        }

        internal WeakDelegate weakFilter;
        public Func<TPayload, bool> Filter
        {
            get { return (Func<TPayload, bool>)weakFilter.Target; }
        }

        public Subscriber(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext, WeakDelegate weakAction)
            : base(subscriptionToken, synchronizationContext)
        {
            this.weakAction = weakAction ?? throw new ArgumentNullException(nameof(weakAction));

            var defaultFilter = new Func<TPayload, bool>(_ => true);
            this.weakFilter = new WeakDelegate(defaultFilter);
        }

        public void Invoke(Action<TPayload> action, TPayload payload)
        {
            switch (executionStrategy)
            {
                case ExecutionStrategyType.PublisherThread:
                    action(payload);
                    break;
                case ExecutionStrategyType.UIThread:
                    if (this.synchronizationContext == null) { throw new InvalidOperationException($"Cannot invoke the action with \"{executionStrategy}\". The Synchronization is null"); }

                    this.SynchronizationContext.Post(_ => action(payload), payload);
                    break;
                case ExecutionStrategyType.BackgroundThread:
                    Task.Run(() => action(payload));
                    break;
            }
        }
    }
}
