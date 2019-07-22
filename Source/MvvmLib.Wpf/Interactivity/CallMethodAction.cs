using System;
using System.Reflection;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to call a method with parameter.
    /// </summary>
    public class CallMethodAction : TriggerAction
    {
        private BindingFlags bindingFlags;

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
            DependencyProperty.Register("TargetObject", typeof(object), typeof(CallMethodAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("MethodName", typeof(string), typeof(CallMethodAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("Parameter", typeof(object), typeof(CallMethodAction), new PropertyMetadata(null));

        /// <summary>
        /// Creates the <see cref="CallMethodAction"/>.
        /// </summary>
        public CallMethodAction()
        {
            this.bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }

        /// <summary>
        /// Creates the <see cref="CallMethodAction"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new CallMethodAction();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected override void Invoke()
        {
            if (TargetObject == null)
                throw new ArgumentException("No TargetObject provided");
            if (MethodName == null)
                throw new ArgumentException("No MethodName provided");

            var target = TargetObject;
            var type = target.GetType();
            if (Parameter == null)
            {
                var method = type.GetMethod(MethodName, bindingFlags);
                if (method == null)
                    throw new ArgumentException($"No method '{MethodName}' found for the type '{type.FullName}'");

                method.Invoke(target, null);
            }
            else
            {
                var parameterType = Parameter.GetType();
                var method = type.GetMethod(MethodName, bindingFlags, null, new Type[] { parameterType }, null);
                if (method == null)
                    throw new ArgumentException($"No method '{MethodName}' with the parameter type '{parameterType}' found for the type '{type.FullName}'");

                method.Invoke(target, new object[] { Parameter });
            }
        }
    }
}
