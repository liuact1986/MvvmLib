using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to wrap a model. Edit, validate and notify the UI of changes.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class ModelWrapper<TModel> : ValidatableBase where TModel : class
    {
        private readonly Dictionary<string, PropertyInfo> propertyCache;

        private TModel model;
        /// <summary>
        /// The model.
        /// </summary>
        public TModel Model
        {
            get { return model; }
        }

        /// <summary>
        /// Creates the model wrapper.
        /// </summary>
        /// <param name="model">The model to wrap</param>
        public ModelWrapper(TModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            this.propertyCache = new Dictionary<string, PropertyInfo>();

            this.model = model;
            this.editor = new ObjectEditor();
            this.Validator = new ObjectValidator(model, (propertyName) => DoCustomValidations(propertyName));
        }

        private PropertyInfo GetProperty(string propertyName)
        {
            if (propertyCache.TryGetValue(propertyName, out PropertyInfo property))
            {
                return property;
            }
            else
            {
                property = typeof(TModel).GetProperty(propertyName);
                if (property == null)
                    throw new ArgumentException($"Property \"{propertyName}\" not found  on \"{typeof(TModel).Name}\"");

                propertyCache[propertyName] = property;
                return property;
            }
        }

        /// <summary>
        /// Gets the value for the property.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="propertyName">The property name</param>
        /// <returns>The value</returns>
        protected TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            var value = property.GetValue(Model);
            return (TValue)value;
        }

        /// <summary>
        /// Sets the value for the property.
        /// </summary>
        /// <typeparam name="TValue">The value type</typeparam>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if value updated</returns>
        protected virtual bool SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            var property = GetProperty(propertyName);
            var oldValue = property.GetValue(Model);
            if (Equals(oldValue, value))
            {
                return false;
            }
            else
            {
                property.SetValue(Model, value);
                OnPropertyChanged(propertyName);
                if (CanValidateOnPropertyChanged)
                    ValidateProperty(propertyName, value);

                return true;
            }
        }

        /// <summary>
        /// Begins edition.
        /// </summary>
        public override void BeginEdit()
        {
            this.editor.Store(this.model);
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
