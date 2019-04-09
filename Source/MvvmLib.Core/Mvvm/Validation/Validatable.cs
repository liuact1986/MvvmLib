using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    public enum ValidationHandling
    {
        OnPropertyChange,
        OnSubmit,
        Explicit
    }

    public class BindableErrorContainer
    {
        private List<string> emptyList = new List<string>();
        private Dictionary<string, List<string>> errorsByPropertyName = new Dictionary<string, List<string>>();

        public List<string> this[string propertyName]
        {
            get => this.errorsByPropertyName.ContainsKey(propertyName) ? 
                this.errorsByPropertyName[propertyName] : emptyList;
        }

        public int Count => this.errorsByPropertyName.Count;

        public bool ContainErrors(string propertyName)
        {
            return errorsByPropertyName.ContainsKey(propertyName);
        }

        public bool ContainError(string propertyName, string error)
        {
            return ContainErrors(propertyName) && errorsByPropertyName[propertyName].Contains(error);
        }

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

    public class Validatable : BindableBase, INotifyDataErrorInfo
    {
        public BindableErrorContainer Errors { get; } = new BindableErrorContainer();

        protected Dictionary<string, PropertyInfo> propertiesCache;

        protected object Source { get; }

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

        private ValidationHandling validationType = ValidationHandling.OnPropertyChange;
        public ValidationHandling ValidationType
        {
            get { return validationType; }
            set
            {
                if (value != validationType)
                {
                    validationType = value;
                    if (CanValidateOnPropertyChanged)
                    {
                        ValidateAll();
                    }
                }
            }
        }

        private bool useDataAnnotations = true;
        public bool UseDataAnnotations
        {
            get { return useDataAnnotations; }
            set
            {
                if (value != UseDataAnnotations)
                {
                    useDataAnnotations = value;
                    if (CanValidateOnPropertyChanged)
                    {
                        ValidateAll();
                    }
                }
            }
        }

        private bool useCustomValidations = true;
        public bool UseCustomValidations
        {
            get { return useCustomValidations; }
            set
            {
                if (value != UseCustomValidations)
                {
                    useCustomValidations = value;
                    if (CanValidateOnPropertyChanged)
                    {
                        ValidateAll();
                    }
                }
            }
        }

        protected bool isSubmitted;
        public bool IsSubmitted
        {
            get { return isSubmitted; }
        }

        public bool CanValidateOnPropertyChanged =>
            ValidationType == ValidationHandling.OnPropertyChange
            || ValidationType == ValidationHandling.OnSubmit && isSubmitted;

        // IDataErrorInfo

        public bool HasErrors => this.Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (this.Errors.ContainErrors(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return null;
            }
        }

        // end IDataErrorInfo

        public Validatable(object model = null)
        {
            this.Source = model ?? (this);
        }

        public void AddPropertyToIgnore(string propertyName)
        {
            if (!this.propertiesToIgnore.Contains(propertyName))
            {
                this.propertiesToIgnore.Add(propertyName);
            }
        }

        public void RemovePropertyToIgnore(string propertyName)
        {
            if (this.propertiesToIgnore.Contains(propertyName))
            {
                this.propertiesToIgnore.Remove(propertyName);
            }
        }

        public bool ContainErrors(string propertyName)
        {
            return Errors.ContainErrors(propertyName);
        }

        public bool ContainError(string propertyName, string error)
        {
            return this.Errors.ContainError(propertyName, error);
        }

        public void AddError(string propertyName, string error)
        {
            if (this.Errors.AddError(propertyName, error))
            {
                this.RaiseErrorsChanged(propertyName);
            }
        }

        public void ClearErrors(string propertyName)
        {
            if (this.Errors.ClearErrors(propertyName))
            {
                this.RaiseErrorsChanged(propertyName);
            }
        }

        public void ClearErrors()
        {
            var properties = GetProperties();
            foreach (var property in properties)
            {
                this.ClearErrors(property.Key);
            }
        }

        protected virtual void RaiseErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            base.RaisePropertyChanged(nameof(HasErrors));
        }

        // validate

        protected void ValidateDataAnnotations(string propertyName, object value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(Source) { MemberName = propertyName };

            if (!Validator.TryValidateProperty(value, context, results))
            {
                foreach (var result in results)
                {
                    AddError(propertyName, result.ErrorMessage);
                }
            }
            else
            {
                this.ClearErrors(propertyName);
            }
        }

        protected virtual IEnumerable<string> DoCustomValidations(string propertyName)
        {
            return null;
        }

        protected void ValidateCustomErrors(string propertyName)
        {
            var errors = DoCustomValidations(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        public void ValidateProperty(string propertyName, object value)
        {
            ClearErrors(propertyName);

            if (UseDataAnnotations)
            {
                ValidateDataAnnotations(propertyName, value);
            }

            if (UseCustomValidations)
            {
                ValidateCustomErrors(propertyName);
            }
        }

        protected Dictionary<string, PropertyInfo> GetProperties()
        {
            if (this.propertiesCache != null)
            {
                return this.propertiesCache;
            }
            else
            {
                var type = Source.GetType();
                var properties = type.GetProperties();
                var result = new Dictionary<string, PropertyInfo>();
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite && !propertiesToIgnore.Contains(property.Name))
                    {
                        result[property.Name] = property;
                    }
                }
                this.propertiesCache = result;
                return result;
            }
        }

        public void ValidateAll()
        {
            var properties = this.GetProperties();
            foreach (var property in properties.Values)
            {
                var value = property.GetValue(Source);
                this.ValidateProperty(property.Name, value);
            }
            this.isSubmitted = true;
        }

        public void Reset()
        {
            this.ClearErrors();
            this.validationType = ValidationHandling.OnPropertyChange;
            this.useCustomValidations = true;
            this.useDataAnnotations = true;
            this.isSubmitted = false;
        }

        protected override bool SetProperty<TValue>(ref TValue storage, TValue value, [CallerMemberName] string propertyName = null)
        {
            var result = base.SetProperty(ref storage, value, propertyName);
            if (result && CanValidateOnPropertyChanged)
            {
                this.ValidateProperty(propertyName, value);
            }
            return result;
        }

    }


}
