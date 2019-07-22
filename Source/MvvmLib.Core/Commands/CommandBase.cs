using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;

namespace MvvmLib.Commands
{
    /// <summary>
    /// The base class for commands.
    /// </summary>
    public abstract class CommandBase : IDelegateCommand
    {
        private readonly HashSet<string> observedProperties;
        private INotifyPropertyChanged inpc;

        /// <summary>
        /// Invoked on can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Creates the <see cref="CommandBase"/>.
        /// </summary>
        public CommandBase()
        {
            observedProperties = new HashSet<string>();
        }

        /// <summary>
        /// Observes the property and raises <see cref="CanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="expression">The property expression</param>
        /// <returns>The command</returns>
        protected internal void ObservePropertyInternal<T>(Expression<Func<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException($"MemberExpression expected. Actual '{expression.Body}'");

            var constantExpression = memberExpression.Expression as ConstantExpression;
            if (constantExpression == null)
                throw new ArgumentException($"ConstantExpression expected. Actual '{memberExpression.Expression}'");

            if (inpc == null)
            {
                // the owner
                var inpc = constantExpression.Value as INotifyPropertyChanged;
                if (inpc == null)
                    throw new InvalidOperationException($"Unable to handle INotifyPropertyChanged for {constantExpression.Value}");

                this.inpc = inpc;
                inpc.PropertyChanged += OnInpcPropertyChanged;
            }

            var propertyName = memberExpression.Member.Name;
            observedProperties.Add(propertyName);
        }

        private void OnInpcPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (observedProperties.Contains(e.PropertyName))
            {
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Unhandles <see cref="INotifyPropertyChanged"/> for observed properties.
        /// </summary>
        public void UnhandleInpcForObservedProperties()
        {
            if (inpc != null)
            {
                inpc.PropertyChanged -= OnInpcPropertyChanged;
            }
        }

        /// <summary>
        /// Observes the property and raises <see cref="RaiseCanExecuteChanged"/> automatically.
        /// </summary>
        /// <typeparam name="T">The type of property</typeparam>
        /// <param name="propertyExpression">The property expression</param>
        public IDelegateCommand ObserveProperty<T>(Expression<Func<T>> propertyExpression)
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
