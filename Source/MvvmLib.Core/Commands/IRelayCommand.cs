using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The relay command interface.
    /// </summary>
    public interface IRelayCommand : ICommand
    {
        /// <summary>
        /// The observed properties.
        /// </summary>
        Dictionary<string, INotifyPropertyChangedObserver> ObservedProperties { get; }


        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        IRelayCommand ObserveProperty<T>(Expression<Func<T>> propertyExpression);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
