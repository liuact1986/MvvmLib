using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    public class Validatable : NotifyDataErrorInfoBase
    {
        public ValidationType ValidationType { get; set; }

        public bool UseDataAnnotations { get; set; }

        public bool UseCustomValidations { get; set; }

        public bool IsSubmitted { get; protected set; }

        public bool CanValidateOnPropertyChanged => ValidationType == ValidationType.OnPropertyChange
            || ValidationType == ValidationType.OnSubmit && IsSubmitted;

        protected object source;

        public Validatable(object model = null)
        {
            this.ValidationType = ValidationType.OnPropertyChange;
            this.UseDataAnnotations = true;
            this.UseCustomValidations = true;
            this.source = model ?? (this);
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

        public void ValidateAll()
        {
            var properties = source.GetType().GetProperties().Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                this.ValidateProperty(property.Name, property.GetValue(source));
            }
            this.IsSubmitted = true;
        }

        protected void ValidateDataAnnotations(string propertyName, object value)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(source) { MemberName = propertyName };

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
