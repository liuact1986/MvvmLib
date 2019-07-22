using MvvmLib.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MvvmLib.Interactivity
{

    /// <summary>
    /// Allows to bind an event to a command.
    /// </summary>
    public class EventToCommandBehavior : Behavior
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
            DependencyProperty.Register("EventName", typeof(string), typeof(EventToCommandBehavior), new PropertyMetadata(null));


        /// <summary>
        /// The command.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// The command.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The command parameter.
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// The command parameter.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The converter.
        /// </summary>
        public IValueConverter Converter
        {
            get { return (IValueConverter)GetValue(ConverterProperty); }
            set { SetValue(ConverterProperty, value); }
        }

        /// <summary>
        /// The converter.
        /// </summary>
        public static readonly DependencyProperty ConverterProperty =
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The converter parameter.
        /// </summary>
        public object ConverterParameter
        {
            get { return (object)GetValue(ConverterParameterProperty); }
            set { SetValue(ConverterParameterProperty, value); }
        }

        /// <summary>
        /// The converter parameter.
        /// </summary>
        public static readonly DependencyProperty ConverterParameterProperty =
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        /// <summary>
        /// The converter culture.
        /// </summary>
        public CultureInfo ConverterCulture
        {
            get { return (CultureInfo)GetValue(ConverterCultureProperty); }
            set { SetValue(ConverterCultureProperty, value); }
        }

        /// <summary>
        /// The converter culture.
        /// </summary>
        public static readonly DependencyProperty ConverterCultureProperty =
            DependencyProperty.Register("ConverterCulture", typeof(CultureInfo), typeof(EventToCommandBehavior), new PropertyMetadata(null));

        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="EventToCommandBehavior"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new EventToCommandBehavior();
        }

        /// <summary>
        /// Binds the event to the command.
        /// </summary>
        protected override void OnAttach()
        {
            HandleEvent();
        }

        /// <summary>
        /// Detaches the behavior.
        /// </summary>
        protected override void OnDetach()
        {
            UnhandleEvent();
        }

        private void HandleEvent()
        {
            if (EventName == null)
                throw new InvalidOperationException("The EventName is not provided");

            EventInfo eventInfo = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (eventInfo == null)
                throw new ArgumentException($"Unable to register the \"{EventName}\" event.");

            MethodInfo methodInfo = typeof(EventToCommandBehavior).GetTypeInfo().GetDeclaredMethod(nameof(OnEvent));
            eventHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            eventInfo.AddEventHandler(associatedObject, eventHandler);
        }

        private void UnhandleEvent()
        {
            if (EventName == null || eventHandler == null)
                return;

            EventInfo eventInfo = associatedObject.GetType().GetRuntimeEvent(EventName);
            if (eventInfo == null)
                throw new ArgumentException($"Unable to unregister the \"{EventName}\" event.");

            eventInfo.RemoveEventHandler(AssociatedObject, eventHandler);
            eventHandler = null;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="eventArgs">The event args</param>
        protected virtual void OnEvent(object sender, object eventArgs)
        {
            if (Command == null)
                return;

            object parameter = null;
            if (Converter != null)
                parameter = Converter.Convert(CommandParameter, typeof(object), ConverterParameter, ConverterCulture);
            else if (CommandParameter != null)
                parameter = CommandParameter;

            if (Command.CanExecute(parameter))
                Command.Execute(parameter);
        }

    }
}
