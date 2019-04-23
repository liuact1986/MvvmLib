using System;

namespace MvvmLib.Message
{
    /// <summary>
    /// The subscription token class.
    /// </summary>
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        private readonly Guid guid;
        private Func<SubscriptionToken, bool> unsubscribe;

        /// <summary>
        /// Creates the subscription token class.
        /// </summary>
        /// <param name="unsubscribe">The unsubscribe function</param>
        public SubscriptionToken(Func<SubscriptionToken, bool> unsubscribe)
        {
            this.guid = Guid.NewGuid();
            this.unsubscribe = unsubscribe;
        }

        /// <summary>
        /// Checks if the guid equals to the othe guid.
        /// </summary>
        /// <param name="other">The other</param>
        /// <returns>True if equals</returns>
        public bool Equals(SubscriptionToken other)
        {
            if (other == null) return false;
            return Equals(guid, other.guid);
        }

        /// <summary>
        /// Dispose the subscription token class.
        /// </summary>
        public void Dispose()
        {
            if (this.unsubscribe != null)
            {
                this.unsubscribe(this);
                this.unsubscribe = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
