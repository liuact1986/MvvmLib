namespace MvvmLib.Message
{
    public class SubscriberOptions
    {
        private readonly Subscriber subscriber;

        public SubscriptionToken Token
        {
            get { return subscriber.SubscriptionToken; }
        }

        public SubscriberOptions(Subscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        public SubscriberOptions WithExecutionStrategy(ExecutionStrategyType executionStrategy)
        {
            subscriber.InvocationStrategy = executionStrategy;
            return this;
        }
    }
}
