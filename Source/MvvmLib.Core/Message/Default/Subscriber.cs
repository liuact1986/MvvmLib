using System;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmLib.Message
{

    public sealed class Subscriber : SubscriberBase
    {
        internal WeakDelegate weakAction;
        public Action Action
        {
            get { return (Action)weakAction.Target; }
        }

        public Subscriber(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext, WeakDelegate weakAction)
            : base(subscriptionToken, synchronizationContext)
        {
            this.weakAction = weakAction ?? throw new ArgumentNullException(nameof(weakAction));
        }

        public void Invoke(Action action)
        {
            switch (executionStrategy)
            {
                case ExecutionStrategyType.PublisherThread:
                    action();
                    break;
                case ExecutionStrategyType.UIThread:
                    if(this.synchronizationContext == null) { throw new InvalidOperationException($"Cannot invoke the action with \"{executionStrategy}\". The Synchronization is null"); }

                    this.SynchronizationContext.Post(_ => action(), null);
                    break;
                case ExecutionStrategyType.BackgroundThread:
                    Task.Run(() => action());
                    break;
            }
        }
    }
}
