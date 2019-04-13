using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    public class ModelWrapper<TModel> : Validatable
    {
        public TModel Model { get; set; }

        public ModelWrapper(TModel model, IEditableObjectService editableService)
            : base(editableService, model)
        {
            Model = model;
        }

        public ModelWrapper(TModel model)
           : this(model, new EditableObjectService())
        { }

        protected PropertyInfo GetProperty(string propertyName)
        {
            var properties = this.GetProperties();
            if (properties.TryGetValue(propertyName, out PropertyInfo property))
            {
                return property;
            }
            return null;
        }

        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            if (property == null) { throw new ArgumentException($"Property \"{propertyName}\" not found in {this.Model.GetType().Name}"); }

            var value = property.GetValue(Model);
            return (TValue)value;
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            if (property == null) { throw new ArgumentException($"Property \"{propertyName}\" not found in {this.Model.GetType().Name}"); }

            property.SetValue(Model, value);
            RaisePropertyChanged(propertyName);
            RaisePropertyChanged(string.Empty);

            if (CanValidateOnPropertyChanged)
            {
                this.ValidateProperty(propertyName, value);
            }
        }
    }

}
