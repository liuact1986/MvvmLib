using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// The Base class for validation and edition.
    /// </summary>
    public abstract class ValidatableBase : INotifyDataErrorInfo, INotifyPropertyChanged, IEditableObject
    {
        /// <summary>
        /// The validator.
        /// </summary>
        protected ObjectValidator Validator;
        /// <summary>
        /// The object editor.
        /// </summary>
        protected ObjectEditor editor;

        private BindableErrorContainer errors;
        /// <summary>
        /// The bindable error container.
        /// </summary>
        public BindableErrorContainer Errors
        {
            get { return errors; }
        }

        private ValidationHandling validationType;
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

                    OnPropertyChanged(nameof(ValidationType));
                }
            }
        }

        /// <summary>
        /// Allows to use or not DataAnnotations for validation (true by default).
        /// </summary>
        public bool UseDataAnnotations
        {
            get { return Validator.UseDataAnnotations; }
            set
            {
                if (value != Validator.UseDataAnnotations)
                {
                    Validator.UseDataAnnotations = value;
                    if (CanValidateOnPropertyChanged)
                        ValidateAll();

                    OnPropertyChanged(nameof(UseDataAnnotations));
                }
            }
        }

        /// <summary>
        /// Allows to use custom validation (true by default).
        /// </summary>
        public bool UseCustomValidations
        {
            get { return Validator.UseCustomValidations; }
            set
            {
                if (value != Validator.UseCustomValidations)
                {
                    Validator.UseCustomValidations = value;
                    if (CanValidateOnPropertyChanged)
                        ValidateAll();

                    OnPropertyChanged(nameof(UseCustomValidations));
                }
            }
        }

        private bool isSubmitted;
        /// <summary>
        /// True when the <see cref="ValidateAll"/> has been invoked.
        /// </summary>
        public bool IsSubmitted
        {
            get { return isSubmitted; }
            private set
            {
                isSubmitted = value;
                OnPropertyChanged(nameof(IsSubmitted));
            }
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
        /// Checks if the model has errors.
        /// </summary>
        public bool HasErrors
        {
            get { return errors.Count > 0; }
        }

        /// <summary>
        /// Invoked on errors changed.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoked on <see cref="Reset"/>.
        /// </summary>
        public event EventHandler Reseted;

        /// <summary>
        /// Invoked on <see cref="CancelEdit"/>.
        /// </summary>
        public event EventHandler EditionCancelled;

        /// <summary>
        /// Creates the validatable and editable base class.
        /// </summary>
        public ValidatableBase()
        {
            this.errors = new BindableErrorContainer();
            this.ValidationType = ValidationHandling.OnPropertyChange;
        }

        /// <summary>
        /// Gets the errors for the property name. 
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The errors of property or null</returns>
        public IEnumerable GetErrors(string propertyName)
        {
            if (this.errors.ContainsErrors(propertyName))
                return this.errors[propertyName];
            else
                return null;
        }

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Notifies that edition was cancelled.
        /// </summary>
        protected void OnEditionCancelled()
        {
            EditionCancelled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Notifies that <see cref="Reset"/> was called.
        /// </summary>
        protected void OnReseted()
        {
            Reseted?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Adds the error for the proeprty name and notify the UI.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        public void AddError(string propertyName, string error)
        {
            if (this.Errors.AddError(propertyName, error))
            {
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Errors));
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Clears the errors for the property name and notify the UI.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void ClearErrors(string propertyName)
        {
            if (errors.ClearErrors(propertyName))
            {
                OnPropertyChanged(nameof(HasErrors));
                OnPropertyChanged(nameof(Errors));
                OnErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Clears all errors and notify the UI.
        /// </summary>
        public void ClearErrors()
        {
            var propertyNames = new List<string>(errors.errorsByProperty.Keys);
            errors.ClearErrors();
            foreach (var propertyName in propertyNames)
                OnErrorsChanged(propertyName);

            OnPropertyChanged(nameof(HasErrors));
            OnPropertyChanged(nameof(Errors));
        }

        /// <summary>
        /// Allows to do custom validations for the property name.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors or null</returns>
        protected virtual IEnumerable<string> DoCustomValidations(string propertyName)
        {
            return null;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        public void ValidateProperty(string propertyName)
        {
            var errors = Validator.ValidateProperty(propertyName);

            ClearErrors(propertyName);

            foreach (var error in errors)
                AddError(propertyName, error);
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to check</param>
        protected void ValidateProperty(string propertyName, object value)
        {
            var errors = Validator.ValidateProperty(propertyName, value);

            ClearErrors(propertyName);

            foreach (var error in errors)
                AddError(propertyName, error);
        }

        /// <summary>
        /// Validates all properties and sets <see cref="IsSubmitted"/> to true.
        /// </summary>
        public void ValidateAll()
        {
            var allErrors = Validator.ValidateAll();

            ClearErrors();

            foreach (var kv in allErrors)
            {
                var propertyName = kv.Key;
                var errors = kv.Value;
                foreach (var error in errors)
                    AddError(propertyName, error);
            }

            this.IsSubmitted = true;
        }

        /// <summary>
        /// Clears the errors and resets <see cref="IsSubmitted"/>.
        /// </summary>
        public void Reset()
        {
            this.ClearErrors();
            this.IsSubmitted = false;
            this.OnReseted();
        }

        /// <summary>
        /// Gets a dictionary with all errors.
        /// </summary>
        /// <returns>A dictionary of errors</returns>
        public Dictionary<string, List<string>> GetErrorSummary()
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var kv in Errors.errorsByProperty)
            {
                var propertyName = kv.Key;
                errors[propertyName] = new List<string>();
                foreach (var error in kv.Value)
                    errors[propertyName].Add(error);
            }
            return errors;
        }

        /// <summary>
        /// Begins edition.
        /// </summary>
        public abstract void BeginEdit();

        /// <summary>
        /// Cancels changes.
        /// </summary>
        public abstract void CancelEdit();

        /// <summary>
        /// Ends edition.
        /// </summary>
        public abstract void EndEdit();
    }

}
