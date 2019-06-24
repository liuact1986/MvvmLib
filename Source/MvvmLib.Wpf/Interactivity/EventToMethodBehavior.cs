using System;
using System.Reflection;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to bind an event to a method.
    /// </summary>
    public class EventToMethodBehavior : NavigationBehavior
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
            DependencyProperty.Register("EventName", typeof(string), typeof(EventToMethodBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The method name.
        /// </summary>
        public string MethodName
        {
            get { return (string)GetValue(MethodNameProperty); }
            set { SetValue(MethodNameProperty, value); }
        }

        /// <summary>
        /// The method name.
        /// </summary>
        public static readonly DependencyProperty MethodNameProperty =
            DependencyProperty.Register("MethodName", typeof(string), typeof(EventToMethodBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The target object.
        /// </summary>
        public object TargetObject
        {
            get { return (object)GetValue(TargetObjectProperty); }
            set { SetValue(TargetObjectProperty, value); }
        }

        /// <summary>
        /// The target object.
        /// </summary>
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register("TargetObject", typeof(object), typeof(EventToMethodBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return (object)GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        /// <summary>
        /// The parameter.
        /// </summary>
        public static readonly DependencyProperty ParameterProperty =
            DependencyProperty.Register("Parameter", typeof(object), typeof(EventToMethodBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="EventToMethodBehavior"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new EventToMethodBehavior();
        }

        /// <summary>
        /// Invoked on attach the associatedObject.
        /// </summary>
        protected override void OnAttach()
        {
            if (!IsInDesignMode(this))
            {
                RegisterEvent();
            }
        }

        private void RegisterEvent()
        {
            if (EventName == null)
                throw new InvalidOperationException("The EventName is not provided");

            EventInfo eventInfo = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (eventInfo == null)
                throw new ArgumentException($"Unable to register the \"{EventName}\" event.");

            MethodInfo methodInfo = typeof(EventToMethodBehavior).GetTypeInfo().GetDeclaredMethod(nameof(OnEvent));
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(associatedObject, eventHandler);
        }

        private void OnEvent(object sender, object eventArgs)
        {
            if (TargetObject != null && MethodName != null)
            {
                var type = TargetObject.GetType();
                if (Parameter == null)
                {
                    var method = type.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (method != null)
                    {
                        method.Invoke(TargetObject, new object[] { });
                    }
                }
                else
                {
                    var method = type.GetMethod(MethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { typeof(object) }, null);
                    if (method != null)
                    {
                        method.Invoke(TargetObject, new object[] { Parameter });
                    }
                }
            }
        }

        /// <summary>
        /// In voked on detach.
        /// </summary>
        protected override void OnDetach()
        {
            UnregisterEvent();
        }

        private void UnregisterEvent()
        {
            if (EventName == null || eventHandler == null)
                return;

            EventInfo eventInfo = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (eventInfo == null)
                throw new ArgumentException($"Unable to unregister the \"{EventName}\" event.");

            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }
    }
}
