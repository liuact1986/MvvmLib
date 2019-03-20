using System;

namespace MvvmLib.Message
{
    public class EmptyEvent : EventBase
    {
        public EventSubscription Subscribe(Action subscriber)
        {
            // subscription without result value
            return this.RegisterSubscriber(subscriber);
        }

        public void Publish()
        {
            this.Clean();
            foreach (var subscription in this.subscriptions)
            {
                var @delegate = subscription.weakDelegate.TryGetDelegate() as Action;
                if (this.UseCurrentSynchronizationContext)
                {
                    this.SynchronizationContext.Post(s => @delegate.DynamicInvoke(), null);
                }
                else
                {
                    @delegate.DynamicInvoke();
                }
            }
        }
    }

}
