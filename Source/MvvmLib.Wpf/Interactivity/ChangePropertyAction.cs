using MvvmLib.Utils;
using System;
using System.Globalization;
using System.Reflection;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to change the value of a property.
    /// </summary>
    public class ChangePropertyAction : TriggerAction
    {
        private PropertyInfo property;

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
            DependencyProperty.Register("TargetObject", typeof(object), typeof(ChangePropertyAction), new PropertyMetadata(null));

        /// <summary>
        /// The property path.
        /// </summary>
        public string PropertyPath
        {
            get { return (string)GetValue(PropertyPathProperty); }
            set { SetValue(PropertyPathProperty, value); }
        }

        /// <summary>
        /// The property path.
        /// </summary>
        public static readonly DependencyProperty PropertyPathProperty =
            DependencyProperty.Register("PropertyPath", typeof(string), typeof(ChangePropertyAction), new PropertyMetadata(null));

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
            DependencyProperty.Register("Value", typeof(object), typeof(ChangePropertyAction), new PropertyMetadata(null));

        /// <summary>
        /// The culture.
        /// </summary>
        public CultureInfo Culture
        {
            get { return (CultureInfo)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
        }

        /// <summary>
        /// The culture.
        /// </summary>
        public static readonly DependencyProperty CultureProperty =
            DependencyProperty.Register("Culture", typeof(CultureInfo), typeof(ChangePropertyAction), new PropertyMetadata(CultureInfo.InvariantCulture));

        /// <summary>
        /// Creates the <see cref="ChangePropertyAction"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ChangePropertyAction();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected override void Invoke()
        {
            if (TargetObject == null)
                throw new ArgumentException("No TargetObject provided");
            if (PropertyPath == null)
                throw new ArgumentException("No PropertyPath provided");

            if (this.property == null)
            {
                this.property = ResolveProperty();
            }

            var value = TypeConversionHelper.TryConvert(property.PropertyType, Value, Culture);
            var owner = PropertyPathHelper.GetOwner(TargetObject, PropertyPath);
            if (owner != null) // a sub property can be null if not instantied
                property.SetValue(owner, value);
        }

        private PropertyInfo ResolveProperty()
        {
            var target = TargetObject;
            var type = target.GetType();
            var property = PropertyPathHelper.GetProperty(type, PropertyPath);
            if (property == null)
                throw new ArgumentException($"Unable to find the property '{PropertyPath}' for the type '{type}'");
            if (!property.CanWrite)
                throw new InvalidOperationException($"Unable to write for  the property '{PropertyPath}' for the type '{type}'");

            return property;
        }
    }
}
