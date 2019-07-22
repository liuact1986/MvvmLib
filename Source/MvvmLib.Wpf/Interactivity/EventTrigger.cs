using System;
using System.Reflection;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A trigger based on events.
    /// </summary>
    public class EventTrigger : TriggerBase
    {
        private Delegate eventHandler;

        /// <summary>
        /// The event name.
        /// </summary>
        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        /// <summary>
        /// The event name.
        /// </summary>
        public static readonly DependencyProperty EventNameProperty =
            DependencyProperty.Register("EventName", typeof(string), typeof(EventTrigger), new PropertyMetadata(null));

        /// <summary>
        /// Creates the <see cref="EventTrigger"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new EventTrigger();
        }

        /// <summary>
        /// Handles the event.
        /// </summary>
        protected override void OnAttach()
        {
            HandleEvent();
        }

        /// <summary>
        /// Unhandles the event.
        /// </summary>
        protected override void OnDetach()
        {
            UnhandleEvent();
        }

        private void HandleEvent()
        {
            if (EventName == null)
                throw new InvalidOperationException("The EventName is not provided");

            EventInfo @event = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (@event == null)
                throw new ArgumentException($"Unable to register the \"{EventName}\" event.");

            MethodInfo method = typeof(EventTrigger).GetTypeInfo().GetDeclaredMethod(nameof(OnEvent));
            eventHandler = method.CreateDelegate(@event.EventHandlerType, this);
            @event.AddEventHandler(associatedObject, eventHandler);
        }

        private void UnhandleEvent()
        {
            if (EventName == null || eventHandler == null)
                return;

            EventInfo @event = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (@event == null)
                throw new ArgumentException($"Unable to unregister the \"{EventName}\" event.");

            @event.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }

        /// <summary>
        /// The method invoked on event.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="eventArgs">The event args</param>
        protected virtual void OnEvent(object sender, object eventArgs)
        {
            InvokeActions();
        }
    }
}
