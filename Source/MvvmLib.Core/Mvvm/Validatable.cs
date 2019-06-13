using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to validate and edit models and view models.
    /// </summary>
    public class Validatable : ValidatableBase
    {
        private List<string> propertiesToIgnore = new List<string>
        {
            "ValidationType",
            "UseDataAnnotations",
            "UseCustomValidations",
            "IsSubmitted",
            "CanValidateOnPropertyChanged",
            "HasErrors"
        };

        /// <summary>
        /// Creates the validatable.
        /// </summary>
        public Validatable()
        {
            this.editor = new ObjectEditor(this.GetType(), propertiesToIgnore);
            this.Validator = new ObjectValidator(this, (propertyName) => DoCustomValidations(propertyName));
        }

        /// <summary>
        /// Sets the property, validates and raises <see cref="INotifyPropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="storage">The storage</param>
        /// <param name="value">The value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if value is updated</returns>
        protected bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                if (CanValidateOnPropertyChanged)
                    ValidateProperty(propertyName, value);

                return true;
            }
        }

        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="expression">The Linq expression</param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                OnPropertyChanged(propertyName);
            }
        }

        /// <summary>
        /// Begins edition.
        /// </summary>
        public override void BeginEdit()
        {
            this.editor.Store(this);
        }

        /// <summary>
        /// Cancels changes.
        /// </summary>
        public override void CancelEdit()
        {
            this.editor.Restore();
            this.Reset();
            this.OnPropertyChanged(string.Empty);
            this.OnEditionCancelled();
        }

        /// <summary>
        /// Ends edition.
        /// </summary>
        public override void EndEdit()
        {
            this.editor.Clean();
        }
    }

}
