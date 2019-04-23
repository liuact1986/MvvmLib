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
        private List<string> emptyList = new List<string>();
        private Dictionary<string, List<string>> errorsByPropertyName = new Dictionary<string, List<string>>();

        /// <summary>
        /// Returns the list of errors or an empty list for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors or an empty list for the property.</returns>
        public List<string> this[string propertyName]
        {
            get => this.errorsByPropertyName.ContainsKey(propertyName) ?
                this.errorsByPropertyName[propertyName] : emptyList;
        }

        /// <summary>
        /// The count of errors by property name.
        /// </summary>
        public int Count => this.errorsByPropertyName.Count;

        /// <summary>
        /// Checks if the property has errors.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has errors</returns>
        public bool ContainErrors(string propertyName)
        {
            return errorsByPropertyName.ContainsKey(propertyName);
        }

        /// <summary>
        /// Checks if the property has already the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the property has already the error</returns>
        public bool ContainError(string propertyName, string error)
        {
            return ContainErrors(propertyName) && errorsByPropertyName[propertyName].Contains(error);
        }

        /// <summary>
        /// Adds an error for the property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the error is added</returns>
        public bool AddError(string propertyName, string error)
        {
            if (!ContainErrors(propertyName))
            {
                this.errorsByPropertyName[propertyName] = new List<string> { error };
                return true;
            }
            else if (!ContainError(propertyName, error))
            {
                this.errorsByPropertyName[propertyName].Add(error);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears the errors for the proeprty.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True the property has errors</returns>
        public bool ClearErrors(string propertyName)
        {
            if (ContainErrors(propertyName))
            {
                errorsByPropertyName.Remove(propertyName);
                return true;
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
        protected ValidationHandling validationType = ValidationHandling.OnPropertyChange;
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
        /// Allows to use or not  <see cref="System.ComponentModel.DataAnnotations"/> for validation, true by default.
        /// </summary>
        protected bool useDataAnnotations = true;
        /// <summary>
        /// Allows to use or not  <see cref="System.ComponentModel.DataAnnotations"/> for validation, true by default.
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
        protected bool useCustomValidations = true;
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

        // IDataErrorInfo


        /// <summary>
        /// True if the source has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return this.errors.Count > 0; }
        }


        /// <summary>
        /// Invoked when errors changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the errors for a property name.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The errors or null</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (this.Errors.ContainErrors(propertyName))
                return Errors[propertyName];
            else
                return null;
        }

        // end IDataErrorInfo

        /// <summary>
        /// Creates the validatable class.
        /// </summary>
        /// <param name="model">The model / source for validation ("this" if null)</param>
        public Validatable(object model = null)
        {
            this.errors = new BindableErrorContainer();
            this.source = model ?? (this);
        }

        /// <summary>
        /// Checks if the property is to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if ignored</returns>
        public bool ContainPropertyToIgnore(string propertyName)
        {
            return this.propertiesToIgnore.Contains(propertyName);
        }

        /// <summary>
        /// Adds a property to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void AddPropertyToIgnore(string propertyName)
        {
            if (!ContainPropertyToIgnore(propertyName))
                this.propertiesToIgnore.Add(propertyName);
        }

        /// <summary>
        /// Removes a property to ignore.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void RemovePropertyToIgnore(string propertyName)
        {
            if (ContainPropertyToIgnore(propertyName))
                this.propertiesToIgnore.Remove(propertyName);
        }

        /// <summary>
        /// Checks if there is errors for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has errors</returns>
        public bool ContainErrors(string propertyName)
        {
            return Errors.ContainErrors(propertyName);
        }

        /// <summary>
        /// Checks if the property has already the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the property has already the error</returns>
        public bool ContainError(string propertyName, string error)
        {
            return this.Errors.ContainError(propertyName, error);
        }

        /// <summary>
        /// Adds an error for the property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        public void AddError(string propertyName, string error)
        {
            if (this.Errors.AddError(propertyName, error))
                this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clears the erros for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void ClearErrors(string propertyName)
        {
            if (this.Errors.ClearErrors(propertyName))
                this.RaiseErrorsChanged(propertyName);
        }

        /// <summary>
        /// Clears the errors for all properties.
        /// </summary>
        public void ClearErrors()
        {
            var properties = GetProperties();
            foreach (var property in properties)
                this.ClearErrors(property.Key);
        }

        /// <summary>
        /// Raises the <see cref="INotifyDataErrorInfo.ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            base.RaisePropertyChanged(nameof(HasErrors));
            base.RaisePropertyChanged(nameof(Errors));
        }


        /// <summary>
        /// Validate with <see cref="System.ComponentModel.DataAnnotations"/>.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to check</param>
        protected void ValidateDataAnnotations(string propertyName, object value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(source) { MemberName = propertyName };

            if (!Validator.TryValidateProperty(value, context, results))
            {
                foreach (var result in results)
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
        protected void ValidateCustomErrors(string propertyName)
        {
            var errors = DoCustomValidations(propertyName);
            if (errors != null)
                foreach (var error in errors)
                    AddError(propertyName, error);
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
                ValidateDataAnnotations(propertyName, value);

            if (UseCustomValidations)
                ValidateCustomErrors(propertyName);
        }

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <returns></returns>
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
        /// Resets all errors and options.
        /// </summary>
        public void Reset()
        {
            this.ClearErrors();
            this.validationType = ValidationHandling.OnPropertyChange;
            this.useCustomValidations = true;
            this.useDataAnnotations = true;
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
