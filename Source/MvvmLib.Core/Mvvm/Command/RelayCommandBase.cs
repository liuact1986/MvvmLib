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
        private readonly Dictionary<string, INotifyPropertyChangedObserver> observedProperties;
        /// <summary>
        /// The observed properties registered with the method ObserveProperty.
        /// </summary>
        public Dictionary<string, INotifyPropertyChangedObserver> ObservedProperties
        {
            get { return observedProperties; }
        }

        /// <summary>
        /// Can execute changed event.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Use the canExecuteMethod to check if the executeMethod can be invoked.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        /// <returns>True if command have to be executed</returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Invokes the executeMethod.
        /// </summary>
        /// <param name="parameter">The parameter</param>
        public abstract void Execute(object parameter);

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Creates the <see cref="RelayCommandBase"/>.
        /// </summary>
        public RelayCommandBase()
        {
            observedProperties = new Dictionary<string, INotifyPropertyChangedObserver>();
        }

        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        /// <returns>The command</returns>
        protected internal void ObservePropertyInternal<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException(nameof(propertyExpression));

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression != null)
            {
                var propertyName = memberExpression.Member.Name;

                if (observedProperties.ContainsKey(propertyName))
                    return;

                var constantExpression = memberExpression.Expression as ConstantExpression;
                if (constantExpression == null)
                    throw new NotSupportedException("Only constant expression is supported for the owner type"); 

                var owner = constantExpression.Value;
                if (owner is INotifyPropertyChanged ownerAsINotifyPropertyChanged)
                {
                    var filter = new Func<INotifyPropertyChanged, PropertyChangedEventArgs, bool>((s, e) => e.PropertyName == propertyName);

                    var propertyChangedObserver = new FilterableNotifyPropertyChangedObserver(ownerAsINotifyPropertyChanged, filter);
                    propertyChangedObserver.SubscribeToPropertyChanged(e => RaiseCanExecuteChanged());

                    observedProperties[propertyName] = propertyChangedObserver;
                }
            }
        }

        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        /// <returns>The command</returns>
        public RelayCommandBase ObserveProperty<T>(Expression<Func<T>> propertyExpression)
        {
            this.ObservePropertyInternal(propertyExpression);
            return this;
        }
    }
}
