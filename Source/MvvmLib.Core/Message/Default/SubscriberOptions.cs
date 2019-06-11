namespace MvvmLib.Message
{

    /// <summary>
    /// Subscriber options class.
    /// </summary>
    public class SubscriberOptions
    {
        private readonly Subscriber subscriber;

        /// <summary>
        /// The token.
        /// </summary>
        public SubscriptionToken Token
        {
            get { return subscriber.SubscriptionToken; }
        }

        /// <summary>
        /// Creates the subscriber options class.
        /// </summary>
        /// <param name="subscriber">The subscriber</param>
        public SubscriberOptions(Subscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        /// <summary>
        /// Allows to change the executionstrategy.
        /// </summary>
        /// <param name="executionStrategy">The execution strategy</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions WithExecutionStrategy(ExecutionStrategyType executionStrategy)
        {
            subscriber.ExecutionStrategy = executionStrategy;
            return this;
        }
    }
}
