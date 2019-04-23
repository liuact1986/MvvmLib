using System;

namespace MvvmLib.Message
{
    /// <summary>
    /// Subscriber options class.
    /// </summary>
    public class SubscriberOptions<TPayload>
    {
        private readonly Subscriber<TPayload> subscriber;


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
        public SubscriberOptions(Subscriber<TPayload> subscriber)
        {
            this.subscriber = subscriber;
        }

        /// <summary>
        /// Allwos to define a filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions<TPayload> WithFilter(Func<TPayload, bool> filter)
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }

            var weakFilter = new WeakDelegate(filter);
            subscriber.weakFilter = weakFilter;

            return this;
        }

        /// <summary>
        /// Allows to change executionstrategy.
        /// </summary>
        /// <param name="executionStrategy">The execution strategy</param>
        /// <returns>The subscriber options</returns>
        public SubscriberOptions<TPayload> WithExecutionStrategy(ExecutionStrategyType executionStrategy)
        {
            subscriber.InvocationStrategy = executionStrategy;
            return this;
        }
    }
}
