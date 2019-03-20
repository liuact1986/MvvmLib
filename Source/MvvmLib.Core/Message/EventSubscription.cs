using System;

namespace MvvmLib.Message
{
    public class EventSubscription
    {
        internal WeakDelegate weakDelegate { get; }

        internal WeakDelegate weakFilter { get; }

        Action<EventSubscription> onUnsubscribe;

        public EventSubscription(Delegate @delegate, Delegate filter, Action<EventSubscription> onUnsubscribe)
        {
            if (filter == null)
            {
                filter = new Predicate<object>((t) => true);
            }

            this.weakDelegate = new WeakDelegate(@delegate);
            this.weakFilter = new WeakDelegate(filter);
            this.onUnsubscribe = onUnsubscribe;
        }

        internal void Kill()
        {
            this.weakDelegate.Kill();
            this.weakFilter.Kill();
        }

        public void Unsubscribe()
        {
            this.Kill();
            this.onUnsubscribe.Invoke(this);
        }
    }

}
