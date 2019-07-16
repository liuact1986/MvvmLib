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
}
