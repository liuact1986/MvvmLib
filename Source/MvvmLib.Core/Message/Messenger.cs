using System;
using System.Collections.Generic;

namespace MvvmLib.Message
{

    public class Messenger : IMessenger
    {
        Dictionary<Type, EventBase> events = new Dictionary<Type, EventBase>();

        public bool IsEventRegistered<TEventType>()
        {
            return this.events.ContainsKey(typeof(TEventType));
        }

        public TEventType GetEvent<TEventType>() where TEventType : EventBase, new()
        {
            lock (events)
            {
                var eventType = typeof(TEventType);
                if (!this.IsEventRegistered<TEventType>())
                {
                    var newEvent = new TEventType();
                    this.events[eventType] = newEvent;
                    return newEvent;
                }
                else
                {
                    return (TEventType)this.events[eventType];
                }
            }
        }

        public bool RemoveEvent<TEventType>()
        {
            if (this.IsEventRegistered<TEventType>())
            {
                var eventType = typeof(TEventType);
                this.events[eventType].UnsubscribeAll();
                this.events.Remove(eventType);
                return true;
            }
            return false;
        }
    }

}
