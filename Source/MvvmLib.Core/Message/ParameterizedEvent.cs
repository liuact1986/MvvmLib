using System;

namespace MvvmLib.Message
{
    public class ParameterizedEvent<TPayload> : EventBase
    {
        public EventSubscription Subscribe(Action<TPayload> subscriber)
        {
            return this.Subscribe(subscriber, new Predicate<TPayload>((t) => true));
        }

        public EventSubscription Subscribe(Action<TPayload> subscriber, Predicate<TPayload> filter)
        {
            return this.RegisterSubscriber(subscriber, filter);
        }

        public void Publish(TPayload parameter)
        {
            this.Clean();
            foreach (var subscription in this.subscriptions)
            {
                var filter = subscription.weakFilter.TryGetDelegate() as Predicate<TPayload>;
                if (filter != null)
                {
                    if (filter.Invoke(parameter))
                    {
                        var @delegate = subscription.weakDelegate.TryGetDelegate() as Action<TPayload>;
                        if (this.UseCurrentSynchronizationContext)
                        {
                            this.SynchronizationContext.Post(s => @delegate.DynamicInvoke(parameter), parameter);
                        }
                        else
                        {
                            @delegate.DynamicInvoke(parameter);
                        }
                    }
                }
            }
        }
    }

}
