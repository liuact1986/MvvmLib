using System.Threading;

namespace MvvmLib.Message
{
    /// <summary>
    /// Base class for <see cref="Subscriber"/> and <see cref="Subscriber{TPayload}"/>.
    /// </summary>
    public class SubscriberBase
    {
        private SynchronizationContext synchronizationContext;
        /// <summary>
        /// The synchronization context.
        /// </summary>
        public SynchronizationContext SynchronizationContext
        {
            get { return synchronizationContext; }
        }

        private readonly SubscriptionToken subscriptionToken;
        /// <summary>
        /// The <see cref="SubscriptionToken"/>.
        /// </summary>
        public SubscriptionToken SubscriptionToken
        {
            get { return subscriptionToken; }
        }

        private ExecutionStrategyType executionStrategy;
        /// <summary>
        /// The execution strategy.
        /// </summary>
        public ExecutionStrategyType ExecutionStrategy
        {
            get { return executionStrategy; }
            set { executionStrategy = value; }
        }

        /// <summary>
        /// Creates the <see cref="SubscriberBase"/> class.
        /// </summary>
        /// <param name="subscriptionToken">The subscription token</param>
        /// <param name="synchronizationContext">The synchronization context</param>
        public SubscriberBase(SubscriptionToken subscriptionToken, SynchronizationContext synchronizationContext)
        {
            if (subscriptionToken == null)
                throw new System.ArgumentNullException(nameof(subscriptionToken));

            this.executionStrategy = ExecutionStrategyType.PublisherThread;
            this.subscriptionToken = subscriptionToken;
            this.synchronizationContext = synchronizationContext;
        }
    }
}
