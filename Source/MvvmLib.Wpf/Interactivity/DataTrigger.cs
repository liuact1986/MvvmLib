using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A trigger based on Data changes.
    /// </summary>
    public class DataTrigger : TriggerBase
    {
        /// <summary>
        /// The binding.
        /// </summary>
        public object Binding
        {
            get { return (object)GetValue(BindingProperty); }
            set { SetValue(BindingProperty, value); }
        }

        /// <summary>
        /// The binding.
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register("Binding", typeof(object), typeof(DataTrigger), new PropertyMetadata(null, OnBindingChanged));

        private static void OnBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as DataTrigger;
            trigger.CompareAndInvokeActions();
        }

        /// <summary>
        /// The value.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// The value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(DataTrigger), new PropertyMetadata(null, OnValueChanged));

        /// <summary>
        /// The comparison. <see cref="ComparisonOperator.Equal"/> by default.
        /// </summary>
        public ComparisonOperator Comparison
        {
            get { return (ComparisonOperator)GetValue(ComparisonProperty); }
            set { SetValue(ComparisonProperty, value); }
        }

        /// <summary>
        /// The comparison. <see cref="ComparisonOperator.Equal"/> by default.
        /// </summary>
        public static readonly DependencyProperty ComparisonProperty =
            DependencyProperty.Register("Comparison", typeof(ComparisonOperator), typeof(DataTrigger), new PropertyMetadata(ComparisonOperator.Equal));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var trigger = d as DataTrigger;
            if (trigger.Binding == null)
                return;

            trigger.CompareAndInvokeActions();
        }

        /// <summary>
        /// Creates the <see cref="DataTrigger"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new DataTrigger();
        }

        /// <summary>
        /// Compares and invoke the actions.
        /// </summary>
        protected virtual void CompareAndInvokeActions()
        {
            if (Compare())
            {
                this.InvokeActions();
            }
        }

        /// <summary>
        /// Compares the binding and the value with the comparison operator.
        /// </summary>
        /// <returns>True invoke actions is required</returns>
        protected bool Compare()
        {
            if (Comparison == ComparisonOperator.Equal)
            {
                return Equals(Binding, Value);
            }
            else
            {
                return !Equals(Binding, Value);
            }
        }

        /// <summary>
        /// Invoked on attach.
        /// </summary>
        protected override void OnAttach()
        {

        }

        /// <summary>
        /// Invoked on detach.
        /// </summary>
        protected override void OnDetach()
        {

        }
    }


    /// <summary>
    /// The comparison operator.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// Equal.
        /// </summary>
        Equal,
        /// <summary>
        /// NotEqual.
        /// </summary>
        NotEqual
    }
}
