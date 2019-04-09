using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    public class ModelWrapper<TModel> : Validatable, IEditableObject
    {
        public TModel Model { get; set; }

        protected IEditableObjectService editableObjectService;

        public ModelWrapper(TModel model, IEditableObjectService editableObjectService)
            : base(model)
        {
            Model = model;
            this.editableObjectService = editableObjectService;
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

            if (ValidationType == ValidationHandling.OnPropertyChange)
            {
                this.ValidateProperty(propertyName, value);
            }
        }

        #region Editable 

        public void BeginEdit()
        {
            this.editableObjectService.Store(Model);
        }

        public void CancelEdit()
        {
            this.editableObjectService.Restore(Model);
            this.Reset();
            this.RaisePropertyChanged(string.Empty);
        }

        public void EndEdit()
        {
            this.editableObjectService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }

        #endregion // Editable
    }


}
