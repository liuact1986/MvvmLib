using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class RelayCommandBase : IRelayCommand
    {
        private readonly Dictionary<string, INotifyPropertyChangedObserver> observedProperties = new Dictionary<string, INotifyPropertyChangedObserver>();

        /// <summary>
        /// Can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Checks if commands have to be executed.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if all commands can execute</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes the execute command.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Notify that the can execute method have to be executed.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        /// <returns>The command</returns>
        protected internal void ObservePropertyInternal<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                var propertyName = memberExpression.Member.Name;

                if (observedProperties.ContainsKey(propertyName))
                    return;

                var constantExpression = memberExpression.Expression as ConstantExpression;
                if (constantExpression == null) { throw new NotSupportedException("Only constant expression is supported for the owner type"); }

                var owner = constantExpression.Value;
                if (owner is INotifyPropertyChanged ownerAsINotifyPropertyChanged)
                {
                    var filter = new Func<INotifyPropertyChanged, PropertyChangedEventArgs, bool>((s, e) => e.PropertyName == propertyName);

                    var propertyChangedObserver = new FilterableNotifyPropertyChangedObserver(ownerAsINotifyPropertyChanged, filter);
                    propertyChangedObserver.SubscribeToPropertyChanged((e) => RaiseCanExecuteChanged());

                    observedProperties[propertyName] = propertyChangedObserver;
                }
            }
        }

        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="expression">The property expression</param>
        /// <returns>The command</returns>
        public RelayCommandBase ObserveProperty<T>(Expression<Func<T>> expression)
        {
            this.ObservePropertyInternal(expression);
            return this;
        }
    }

}
