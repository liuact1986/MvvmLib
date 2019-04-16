using System;

namespace MvvmLib.Message
{
    public class SubscriberOptions<TPayload>
    {
        private readonly Subscriber<TPayload> subscriber;

        public SubscriptionToken Token
        {
            get { return subscriber.SubscriptionToken; }
        }

        public SubscriberOptions(Subscriber<TPayload> subscriber)
        {
            this.subscriber = subscriber;
        }

        public SubscriberOptions<TPayload> WithFilter(Func<TPayload, bool> filter)
        {
            if (filter == null) { throw new ArgumentNullException(nameof(filter)); }

            var weakFilter = new WeakDelegate(filter);
            subscriber.weakFilter = weakFilter;

            return this;
        }

        public SubscriberOptions<TPayload> WithExecutionStrategy(ExecutionStrategyType executionStrategy)
        {
            subscriber.InvocationStrategy = executionStrategy;
            return this;
        }
    }
}
