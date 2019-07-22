using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to invoke a command.
    /// </summary>
    public class InvokeCommandAction : TriggerAction
    {
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
            DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("Converter", typeof(IValueConverter), typeof(InvokeCommandAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("ConverterParameter", typeof(object), typeof(InvokeCommandAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("ConverterCulture", typeof(CultureInfo), typeof(InvokeCommandAction), new PropertyMetadata(null));


        /// <summary>
        /// Creates the <see cref="InvokeCommandAction"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new InvokeCommandAction();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected override void Invoke()
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
