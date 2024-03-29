﻿using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Base class for Models or ViewModels that require to notify property changed <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Sets the value of the property and raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event handler.
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="storage">The field</param>
        /// <param name="value">The new value</param>
        /// <param name="propertyName">The property name</param>
        /// <returns>True if the value has changed</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;
            else
            {
                storage = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        /// <summary>
        /// Notifies that the property changed.
        /// </summary>
        /// <param name="propertyName">The property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        /// <summary>
        /// Notifies that the property changed.
        /// </summary>
        /// <typeparam name="T">The property type</typeparam>
        /// <param name="expression">The Linq expression</param>
        protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null)
            {
                string propertyName = memberExpression.Member.Name;
                OnPropertyChanged(propertyName);
            }
        }
    }
}
