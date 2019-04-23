using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Base class for wrapping a model. Allows to validate and edit properties.
    /// </summary>
    /// <typeparam name="TModel">The model</typeparam>
    public class ModelWrapper<TModel> : Validatable, IEditableObject
    {
        /// <summary>
        /// The model.
        /// </summary>
        public TModel Model { get; set; }

        /// <summary>
        /// The editable object service.
        /// </summary>
        protected IEditableObjectService editableObjectService;

        /// <summary>
        /// Creates the model wrapper class.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="editableObjectService">The editable object service</param>
        public ModelWrapper(TModel model, IEditableObjectService editableObjectService)
            : base(model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            if (editableObjectService == null)
                throw new ArgumentNullException(nameof(editableObjectService));

            Model = model;
            this.editableObjectService = editableObjectService;
        }


        /// <summary>
        /// Creates the model wrapper class.
        /// </summary>
        /// <param name="model">The model</param>
        public ModelWrapper(TModel model)
           : this(model, new EditableObjectService())
        { }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The <see cref="PropertyInfo"/> or null</returns>
        protected PropertyInfo GetProperty(string propertyName)
        {
            var properties = this.GetProperties();
            if (properties.TryGetValue(propertyName, out PropertyInfo property))
                return property;
            
            return null;
        }

        /// <summary>
        /// Gets the value for a property.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <returns>The value</returns>
        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            if (property == null) { throw new ArgumentException($"Property \"{propertyName}\" not found in {this.Model.GetType().Name}"); }

            var value = property.GetValue(Model, null);
            return (TValue)value;
        }

        /// <summary>
        /// Sets the value for a property.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            if (property == null) { throw new ArgumentException($"Property \"{propertyName}\" not found in {this.Model.GetType().Name}"); }

            property.SetValue(Model, value);
            RaisePropertyChanged(propertyName);
            RaisePropertyChanged(string.Empty);

            if (CanValidateOnPropertyChanged)
                this.ValidateProperty(propertyName, value);
        }

        #region Editable 

        /// <summary>
        /// Clones the values fo the model.
        /// </summary>
        public void BeginEdit()
        {
            this.editableObjectService.Store(Model);
        }

        /// <summary>
        /// Reset the values of the model and clear errors.
        /// </summary>
        public void CancelEdit()
        {
            this.editableObjectService.Restore(Model);
            this.Reset();
            this.EndEdit();
        }

        /// <summary>
        /// Clear the cloned values and notify changes.
        /// </summary>
        public void EndEdit()
        {
            this.editableObjectService.Clear();
            this.RaisePropertyChanged(string.Empty);
        }

        #endregion // Editable
    }

}
