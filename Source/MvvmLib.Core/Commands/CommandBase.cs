using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class CommandBase : IRelayCommand
    {
        private readonly Dictionary<string, INotifyPropertyChangedObserver> observedProperties;
        /// <summary>
        /// The observed properties.
        /// </summary>
        public Dictionary<string, INotifyPropertyChangedObserver> ObservedProperties
        {
            get { return observedProperties; }
        }

        /// <summary>
        /// Invoked on can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;


        /// <summary>
        /// Creates the <see cref="CommandBase"/>.
        /// </summary>
        public CommandBase()
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
                    propertyChangedObserver.Subscribe(e => RaiseCanExecuteChanged());

                    observedProperties[propertyName] = propertyChangedObserver;
                }
            }
        }

        /// <summary>
        /// Observes <see cref="INotifyPropertyChanged"/> event for a property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        public IRelayCommand ObserveProperty<T>(Expression<Func<T>> propertyExpression)
        {
            this.ObservePropertyInternal(propertyExpression);
            return this;
        }

        /// <summary>
        /// Checks if the executeMethod can be invoked.
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
    }
}
