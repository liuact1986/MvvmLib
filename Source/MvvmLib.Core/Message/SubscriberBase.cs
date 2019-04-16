using System.Threading;

namespace MvvmLib.Message
{
    public class SubscriberBase
    {
        protected SynchronizationContext synchronizationContext;
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
        }

        protected readonly SubscriptionToken subscriptionToken;
        public SubscriptionToken SubscriptionToken
        {
            get { return subscriptionToken; }
        }

        protected ExecutionStrategyType executionStrategy;
        public ExecutionStrategyType InvocationStrategy
        {
            get { return executionStrategy; }
            set { executionStrategy = value; }
        }

        public SubscriberBase(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext)
        {
            this.executionStrategy = ExecutionStrategyType.PublisherThread;
            this.subscriptionToken = subscriptionToken ?? throw new System.ArgumentNullException(nameof(subscriptionToken));
            this.synchronizationContext = synchronizationContext;
        }
    }
}
