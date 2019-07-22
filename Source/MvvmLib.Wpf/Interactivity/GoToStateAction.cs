using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to go to a state name with the <see cref="VisualStateManager"/>.
    /// </summary>
    public class GoToStateAction : TriggerAction
    {
        /// <summary>
        /// Checks GoTostate success.
        /// </summary>
        protected bool success;
        /// <summary>
        /// Checks GoTostate success.
        /// </summary>
        public bool Success
        {
            get { return success; }
        }

        /// <summary>
        /// The target.
        /// </summary>
        public FrameworkElement Target
        {
            get { return (FrameworkElement)GetValue(TargetProperty); }
            set { SetValue(TargetProperty, value); }
        }

        /// <summary>
        /// The target.
        /// </summary>
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(FrameworkElement), typeof(GoToStateAction), new PropertyMetadata(null));

        /// <summary>
        /// The state name.
        /// </summary>
        public string StateName
        {
            get { return (string)GetValue(StateNameProperty); }
            set { SetValue(StateNameProperty, value); }
        }

        /// <summary>
        /// The state name.
        /// </summary>
        public static readonly DependencyProperty StateNameProperty =
            DependencyProperty.Register("StateName", typeof(string), typeof(GoToStateAction), new PropertyMetadata(null));

        /// <summary>
        /// Use transitions.
        /// </summary>
        public bool UseTransitions
        {
            get { return (bool)GetValue(UseTransitionsProperty); }
            set { SetValue(UseTransitionsProperty, value); }
        }

        /// <summary>
        /// Use transitions.
        /// </summary>
        public static readonly DependencyProperty UseTransitionsProperty =
            DependencyProperty.Register("UseTransitions", typeof(bool), typeof(GoToStateAction), new PropertyMetadata(false));

        /// <summary>
        /// Creates the <see cref="GoToStateAction"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GoToStateAction();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected override void Invoke()
        {
            if (StateName == null)
                throw new ArgumentNullException(nameof(StateName));

            var element = GetElement();
            if (element == null)
                throw new ArgumentException("Unable to resolve target element. No Target provided");

            var success = GoToState(element, StateName, UseTransitions);
            this.success = success;
        }

        private FrameworkElement GetElement()
        {
            if (Target != null)
            {
                return Target;
            }
            if (this.associatedObject is FrameworkElement)
            {
                return (FrameworkElement)this.associatedObject;
            }
            return null;
        }

        /// <summary>
        /// Gets to the state with the <see cref="VisualStateManager"/>.
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="stateName">The state name</param>
        /// <param name="useTransitions">Use transitions</param>
        /// <returns>True if success</returns>
        protected virtual bool GoToState(FrameworkElement element, string stateName, bool useTransitions)
        {
            Control control = element as Control;
            if (control == null)
            {
                return VisualStateManager.GoToElementState(element, stateName, useTransitions);
            }
            else
            {
                control.ApplyTemplate();
                return VisualStateManager.GoToState(control, stateName, useTransitions);
            }
        }
    }
}
