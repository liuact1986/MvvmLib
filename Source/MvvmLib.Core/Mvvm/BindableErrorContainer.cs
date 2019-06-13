using System.Collections.Generic;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to bind errors.
    /// </summary>
    public class BindableErrorContainer
    {
        private static readonly List<string> emptyErrorList = new List<string>();
        internal readonly Dictionary<string, List<string>> errorsByProperty;

        /// <summary>
        /// Returns the list of errors or an empty list for the property.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>The list of errors or an empty list for the property</returns>
        public IReadOnlyList<string> this[string propertyName]
        {
            get
            {
                if (errorsByProperty.TryGetValue(propertyName, out List<string> errorsForPropertyName))
                    return errorsForPropertyName;

                return emptyErrorList;
            }
        }

        /// <summary>
        /// The count of errors by property name.
        /// </summary>
        public int Count
        {
            get { return this.errorsByProperty.Count; }
        }

        /// <summary>
        /// Creates the bindable error container.
        /// </summary>
        public BindableErrorContainer()
        {
            errorsByProperty = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Checks if the property has errors.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the property has errors</returns>
        public bool ContainsErrors(string propertyName)
        {
            return errorsByProperty.ContainsKey(propertyName);
        }

        /// <summary>
        /// Checks if the property has already the error.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the property has already the error</returns>
        public bool ContainsError(string propertyName, string error)
        {
            return ContainsErrors(propertyName) && errorsByProperty[propertyName].Contains(error);
        }

        /// <summary>
        /// Adds an error for the property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <param name="error">The error</param>
        /// <returns>True if the error is added</returns>
        internal bool AddError(string propertyName, string error)
        {
            if (!errorsByProperty.ContainsKey(propertyName))
            {
                errorsByProperty[propertyName] = new List<string> { error };
                return true;
            }
            else
            {
                if (!errorsByProperty[propertyName].Contains(error))
                {
                    errorsByProperty[propertyName].Add(error);
                    return true;
                }
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
            if (errorsByProperty.ContainsKey(propertyName))
            {
                var removed = errorsByProperty.Remove(propertyName);
                return removed;
            }
            return false;
        }

        internal void ClearErrors()
        {
            errorsByProperty.Clear();
        }
    }

}
