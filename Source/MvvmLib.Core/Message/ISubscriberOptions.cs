using System;

namespace MvvmLib.Message
{
    /// <summary>
    /// Subscriber options class.
    /// </summary>
    public interface ISubscriberOptions
    {
        /// <summary>
        /// The token.
        /// </summary>
        SubscriptionToken Token { get; }

        /// <summary>
        /// Allows to unsubscribe to event.
        /// </summary>
        /// <returns>True if unsubscribed</returns>
        bool Unsubscribe();
    }

    /// <summary>
    /// Subscriber options class.
    /// </summary>
    public interface IParameterizedEventSubscriberOptions<TPayload> : ISubscriberOptions
    {
        /// <summary>
        /// Allows to change the executionstrategy.
        /// </summary>
        /// <param name="executionStrategy">The execution strategy</param>
        /// <returns>The subscriber options</returns>
        SubscriberOptions<TPayload> WithExecutionStrategy(ExecutionStrategyType executionStrategy);

        /// <summary>
        /// Allows to define a filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <returns>The subscriber options</returns>
        SubscriberOptions<TPayload> WithFilter(Func<TPayload, bool> filter);
    }
}
