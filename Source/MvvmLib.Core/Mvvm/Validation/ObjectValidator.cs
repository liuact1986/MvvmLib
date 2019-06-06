using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to validate objects.
    /// </summary>
    public class ObjectValidator
    {
        private object source;
        private Dictionary<string, PropertyInfo> propertyCache = new Dictionary<string, PropertyInfo>();
        private Func<string, IEnumerable<string>> doCustomValidations;

        private bool useDataAnnotations;
        /// <summary>
        /// Allows to use or not DataAnnotations for validation (true by default).
        /// </summary>
        public bool UseDataAnnotations
        {
            get { return useDataAnnotations; }
            set { useDataAnnotations = value; }
        }

        private bool useCustomValidations;
        /// <summary>
        /// Allows to use custom validation (true by default).
        /// </summary>
        public bool UseCustomValidations
        {
            get { return useCustomValidations; }
            set { useCustomValidations = value; }
        }

        /// <summary>
        /// Creates the validator.
        /// </summary>
        /// <param name="source">The source (model, etc.)</param>
        /// <param name="doCustomValidations">The custom validation function</param>
        public ObjectValidator(object source, Func<string, IEnumerable<string>> doCustomValidations)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (doCustomValidations == null)
                throw new ArgumentNullException(nameof(doCustomValidations));

            this.useDataAnnotations = true;
            this.useCustomValidations = true;

            this.source = source;
            this.doCustomValidations = doCustomValidations;

            var properties = source.GetType().GetProperties();
            foreach (var property in properties)
            {
                propertyCache[property.Name] = property;
            }
        }

        private PropertyInfo GetProperty(string propertyName)
        {
            if (propertyCache.TryGetValue(propertyName, out PropertyInfo property))
            {
                return property;
            }
            throw new InvalidOperationException($"No property found for \"{propertyName}\" in \"{source.GetType().Name}\"");
        }

        private void ValidateByDataAnnotations(string propertyName, object value, List<string> errors)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(source) { MemberName = propertyName };

            if (!Validator.TryValidateProperty(value, context, validationResults))
            {
                foreach (var result in validationResults)
                    if (!errors.Contains(result.ErrorMessage))
                        errors.Add(result.ErrorMessage);
            }
        }

        private void ValidateByCustomValidations(string propertyName, List<string> errors)
        {
            var customValidationErrors = doCustomValidations(propertyName);
            if (customValidationErrors != null)
            {
                foreach (var error in customValidationErrors)
                    if (!errors.Contains(error))
                        errors.Add(error);
            }
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="value">The value to check</param>
        /// <returns>The list of errors</returns>
        public List<string> ValidateProperty(string propertyName, object value)
        {
            var errors = new List<string>();

            if (useDataAnnotations)
            {
                ValidateByDataAnnotations(propertyName, value, errors);
            }
            if (useCustomValidations)
            {
                ValidateByCustomValidations(propertyName, errors);
            }

            return errors;
        }

        /// <summary>
        /// Validates the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors</returns>
        public List<string> ValidateProperty(string propertyName)
        {
            var property = GetProperty(propertyName);
            var value = property.GetValue(source);

            return ValidateProperty(propertyName, value);
        }

        /// <summary>
        /// Validates all properties.
        /// </summary>
        /// <returns>A dictionary of errors</returns>
        public Dictionary<string, List<string>> ValidateAll()
        {
            var allErrors = new Dictionary<string, List<string>>();

            foreach (var property in propertyCache.Values)
            {
                var propertyName = property.Name;
                var errors = new List<string>();
                if (useDataAnnotations)
                {
                    var value = property.GetValue(source);
                    ValidateByDataAnnotations(propertyName, value, errors);
                }
                if (useCustomValidations)
                {
                    ValidateByCustomValidations(propertyName, errors);
                }

                if (errors.Count > 0)
                    allErrors[propertyName] = errors;
            }

            return allErrors;
        }
    }

}
