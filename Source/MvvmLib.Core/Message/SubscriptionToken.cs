using System;

namespace MvvmLib.Message
{
    public class SubscriptionToken : IEquatable<SubscriptionToken>, IDisposable
    {
        private readonly Guid guid;
        private Func<SubscriptionToken, bool> unsubscribe;

        public SubscriptionToken(Func<SubscriptionToken, bool> unsubscribe)
        {
            this.guid = Guid.NewGuid();
            this.unsubscribe = unsubscribe;
        }

        public bool Equals(SubscriptionToken other)
        {
            if (other == null) return false;
            return Equals(guid, other.guid);
        }

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
