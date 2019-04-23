using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Base class for models and view models. Implements <see cref="INotifyPropertyChanged"/>. Allows to notify that a property has changed.
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed event handler.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of the property.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="storage">The field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the value has changed</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                RaisePropertyChanged(propertyName);
                return true;
            }
        }

        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="expression">The Linq expression</param>
        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                RaisePropertyChanged(propertyName);
            }
        }
    }
}
