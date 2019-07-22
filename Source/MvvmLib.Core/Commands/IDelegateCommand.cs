using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The relay command interface.
    /// </summary>
    public interface IDelegateCommand : ICommand
    {
        /// <summary>
        /// Observes the property and raises CanExecuteChanged automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        IDelegateCommand ObserveProperty<T>(Expression<Func<T>> propertyExpression);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Unhandles <see cref="INotifyPropertyChanged"/> for observed properties.
        /// </summary>
        void UnhandleInpcForObservedProperties();
    }
}
