using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// The validation type.
    /// </summary>
    public enum ValidationHandling
    {
        /// <summary>
        /// On property changed
        /// </summary>
        OnPropertyChange,
        /// <summary>
        /// After ValidateAll invoked
        /// </summary>
        OnSubmit,
        /// <summary>
        /// Only with ValidateAll and ValidateProperty invoked
        /// </summary>
        Explicit
    }

    /// <summary>
    /// Allows to bind errors.
    /// </summary>
    public class BindableErrorContainer
    {
        private static readonly List<string> emptyErrorList = new List<string>();
        private readonly Dictionary<string, List<string>> errorsByPropertyName;

        /// <summary>
        /// Returns the list of errors or an empty list for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors or an empty list for the property</returns>
        public IReadOnlyList<string> this[string propertyName]
        {
            get
            {
                if (errorsByPropertyName.TryGetValue(propertyName, out List<string> errorsForPropertyName))
                    return errorsForPropertyName;

                return emptyErrorList;
            }
        }

        /// <summary>
        /// The count of errors by property name.
        /// </summary>
        public int Count
        {
            get { return this.errorsByPropertyName.Count; }
        }

        /// <summary>
        /// Creates the bindable error container.
        /// </summary>
        public BindableErrorContainer()
        {
            errorsByPropertyName = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Checks if the property has errors.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has errors</returns>
        public bool ContainsErrors(string propertyName)
        {
            return errorsByPropertyName.ContainsKey(propertyName);
        }

        /// <summary>
        /// Checks if the property has already the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the property has already the error</returns>
        public bool ContainsError(string propertyName, string error)
        {
            return ContainsErrors(propertyName) && errorsByPropertyName[propertyName].Contains(error);
        }

        /// <summary>
        /// Adds an error for the property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the error is added</returns>
        internal bool AddError(string propertyName, string error)
        {
            var hasErrorsForProperty = errorsByPropertyName.ContainsKey(propertyName);
            if (!hasErrorsForProperty)
                errorsByPropertyName[propertyName] = new List<string>();

            if (!hasErrorsForProperty || !errorsByPropertyName[propertyName].Contains(error))
            {
                errorsByPropertyName[propertyName].Add(error);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the errors for the proeprty.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True the property has errors</returns>
        internal bool ClearErrors(string propertyName)
        {
            if (errorsByPropertyName.ContainsKey(propertyName))
            {
                var removed = errorsByPropertyName.Remove(propertyName);
                return removed;
            }
            return false;
        }
    }

    /// <summary>
    /// Implements <see cref="INotifyDataErrorInfo"/>. Allows to validate properties with <see cref="System.ComponentModel.DataAnnotations"/> or cutom validations.
    /// </summary>
    public class Validatable : BindableBase, INotifyDataErrorInfo
    {
        /// <summary>
        /// The source.
        /// </summary>
        protected object source;

        /// <summary>
        /// The property cache.
        /// </summary>
        protected Dictionary<string, PropertyInfo> propertyCache;

        /// <summary>
        /// The error container.
        /// </summary>
        protected BindableErrorContainer errors;

        /// <summary>
        /// The error container.
        /// </summary>
        public BindableErrorContainer Errors
        {
            get { return errors; }
        }

        /// <summary>
        /// True if the source has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return this.errors.Count > 0; }
        }

        /// <summary>
        /// The list of properties to ignore.
        /// </summary>
        protected List<string> propertiesToIgnore = new List<string>
        {
            // public properties
            "Errors",
            "ValidationType",
            "UseDataAnnotations",
            "UseCustomValidations",
            "IsSubmitted",
            "CanValidateOnPropertyChanged",
            "HasErrors"
        };

        /// <summary>
        /// The validation type, <see cref="ValidationHandling.OnPropertyChange"/> by default.
        /// </summary>
        protected ValidationHandling validationType;
        /// <summary>
        /// The validation type, <see cref="ValidationHandling.OnPropertyChange"/> by default.
        /// </summary>
        public ValidationHandling ValidationType
        {
            get { return validationType; }
            set
            {
                if (value != validationType)
                {
                    validationType = value;
                    if (CanValidateOnPropertyChanged)
                        ValidateAll();
                }
            }
        }

        /// <summary>
        /// Allows to use or not <see cref="System.ComponentModel.DataAnnotations"/> for validation, true by default.
        /// </summary>
        protected bool useDataAnnotations;
        /// <summary>
        /// Allows to use or not <see cref="System.ComponentModel.DataAnnotations"/> for validation, true by default.
        /// </summary>
        public bool UseDataAnnotations
        {
            get { return useDataAnnotations; }
            set
            {
                if (value != UseDataAnnotations)
                {
                    useDataAnnotations = value;
                    if (CanValidateOnPropertyChanged)
                        ValidateAll();
                }
            }
        }

        /// <summary>
        /// Allows to use custom validation, true by default.
        /// </summary>
        protected bool useCustomValidations;
        /// <summary>
        /// Allows to use custom validation, true by default.
        /// </summary>
        public bool UseCustomValidations
        {
            get { return useCustomValidations; }
            set
            {
                if (value != UseCustomValidations)
                {
                    useCustomValidations = value;
                    if (CanValidateOnPropertyChanged)
                        ValidateAll();
                }
            }
        }

        /// <summary>
        /// True when the <see cref="ValidateAll"/> has been invoked.
        /// </summary>
        protected bool isSubmitted;

        /// <summary>
        /// True when the <see cref="ValidateAll"/> has been invoked.
        /// </summary>
        public bool IsSubmitted
        {
            get { return isSubmitted; }
        }

        /// <summary>
        /// Checks if can validate when a property has changed.
        /// </summary>
        public bool CanValidateOnPropertyChanged
        {
            get
            {
                var canValidateOnPropertyChanged = ValidationType == ValidationHandling.OnPropertyChange
                    || (ValidationType == ValidationHandling.OnSubmit && isSubmitted);
                return canValidateOnPropertyChanged;
            }
        }

        /// <summary>
        /// Invoked when errors changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Creates the validatable class.
        /// </summary>
        /// <param name="model">The model / source for validation ("this" if null)</param>
        public Validatable(object model = null)
        {
            this.errors = new BindableErrorContainer();
            this.validationType = ValidationHandling.OnPropertyChange;
            this.useDataAnnotations = true;
            this.useCustomValidations = true;
            this.source = model ?? (this);
        }

        /// <summary>
        /// Checks if the property is to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if ignored</returns>
        public bool ContainsPropertyToIgnore(string propertyName)
        {
            return this.propertiesToIgnore.Contains(propertyName);
        }

        /// <summary>
        /// Adds a property to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void AddPropertyToIgnore(string propertyName)
        {
            if (!ContainsPropertyToIgnore(propertyName))
                this.propertiesToIgnore.Add(propertyName);
        }

        /// <summary>
        /// Removes a property to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void RemovePropertyToIgnore(string propertyName)
        {
            if (ContainsPropertyToIgnore(propertyName))
                this.propertiesToIgnore.Remove(propertyName);
        }

        /// <summary>
        /// Raises the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the errors for a property name.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The errors or null</returns>
        public virtual IEnumerable GetErrors(string propertyName)
        {
            if (this.Errors.ContainsErrors(propertyName))
                return Errors[propertyName];
            else
                return null;
        }

        /// <summary>
        /// Validate with <see cref="System.ComponentModel.DataAnnotations"/>.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to check</param>
        protected virtual void ValidateByDataAnnotations(string propertyName, object value)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(source) { MemberName = propertyName };

            if (!Validator.TryValidateProperty(value, context, validationResults))
            {
                foreach (var result in validationResults)
                    AddError(propertyName, result.ErrorMessage);
            }
            else
                this.ClearErrors(propertyName);
        }

        /// <summary>
        /// Allows to validate with custom validations.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors or null</returns>
        protected virtual IEnumerable<string> DoCustomValidations(string propertyName)
        {
            return null;
        }

        /// <summary>
        /// Validates with custom validations.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void ValidateByCustomValidations(string propertyName)
        {
            var errors = DoCustomValidations(propertyName);
            if (errors != null)
                foreach (var error in errors)
                    AddError(propertyName, error);
        }

        /// <summary>
        /// Clears the erros for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public virtual void ClearErrors(string propertyName)
        {
            if (errors.ClearErrors(propertyName))
            {
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Errors));
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Clears the errors for all properties.
        /// </summary>
        public virtual void ClearErrors()
        {
            var properties = GetProperties();
            foreach (var property in properties)
                this.ClearErrors(property.Key);
        }

        /// <summary>
        /// Adds an error for the property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        public virtual void AddError(string propertyName, string error)
        {
            if (this.Errors.AddError(propertyName, error))
            {
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Errors));
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns>A dictionary with property name and property info</returns>
        protected Dictionary<string, PropertyInfo> GetProperties()
        {
            if (this.propertyCache != null)
                return this.propertyCache;
            else
            {
                var type = source.GetType();
                var properties = type.GetProperties();
                var result = new Dictionary<string, PropertyInfo>();
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite && !propertiesToIgnore.Contains(property.Name))
                        result[property.Name] = property;
                }
                this.propertyCache = result;
                return result;
            }
        }

        /// <summary>
        /// Validates a propeprty.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to check</param>
        public void ValidateProperty(string propertyName, object value)
        {
            ClearErrors(propertyName);

            if (UseDataAnnotations)
                ValidateByDataAnnotations(propertyName, value);

            if (UseCustomValidations)
                ValidateByCustomValidations(propertyName);
        }

        /// <summary>
        /// Validates all properties and sets <see cref="IsSubmitted"/> to true.
        /// </summary>
        public void ValidateAll()
        {
            var properties = this.GetProperties();
            foreach (var property in properties.Values)
            {
                var value = property.GetValue(source);
                this.ValidateProperty(property.Name, value);
            }
            this.isSubmitted = true;
        }

        /// <summary>
        /// Clear errors and reste is submitted.
        /// </summary>
        public void Reset()
        {
            this.ClearErrors();
            this.isSubmitted = false;
        }

        /// <summary>
        /// Sets the property, validates if <see cref="CanValidateOnPropertyChanged"/> and raises <see cref="INotifyPropertyChanged"/> event.
        /// </summary>
        /// <typeparam name="TValue">The type of value</typeparam>
        /// <param name="storage">The storage</param>
        /// <param name="value">The value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if value is updated</returns>
        protected override bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            if (result && CanValidateOnPropertyChanged)
                this.ValidateProperty(propertyName, value);

            return result;
        }
    }

}
