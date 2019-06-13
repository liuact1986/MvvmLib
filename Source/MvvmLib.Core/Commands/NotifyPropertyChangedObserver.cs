using System;
using System.ComponentModel;

namespace MvvmLib.Commands
{
    /// <summary>
    /// Allows to subscribe and notify on property changed for an object that implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public class NotifyPropertyChangedObserver : INotifyPropertyChangedObserver
    {
        /// <summary>
        /// The owner class.
        /// </summary>
        protected readonly INotifyPropertyChanged owner;

        /// <summary>
        /// The callback.
        /// </summary>
        protected Action<PropertyChangedEventArgs> onPropertyChangedCallback;

        /// <summary>
        /// Creates the property changed observer.
        /// </summary>
        /// <param name="owner">The owner class to observe</param>
        public NotifyPropertyChangedObserver(INotifyPropertyChanged owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>
        /// Subscribe to property changed of the owner.
        /// </summary>
        /// <param name="onPropertyChangedCallback">The callback invoked on property changed</param>
        public virtual void SubscribeToPropertyChanged(Action<PropertyChangedEventArgs> onPropertyChangedCallback)
        {
            this.onPropertyChangedCallback = onPropertyChangedCallback;
            this.owner.PropertyChanged += OnPropertyChanged;
        }

        /// <summary>
        /// Unsubscribe to property changed of the owner.
        /// </summary>
        public virtual void UnsubscribeToPropertyChanged()
        {
            this.onPropertyChangedCallback = null;
            this.owner.PropertyChanged -= OnPropertyChanged;
        }

        /// <summary>
        /// The method invoked on property changed to notify the subscribers.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.onPropertyChangedCallback?.Invoke(e);
        }
    }

    /// <summary>
    /// Allows to filter on property changed with the base class <see cref="NotifyPropertyChangedObserver"/>.
    /// </summary>
    public class FilterableNotifyPropertyChangedObserver : NotifyPropertyChangedObserver
    {
        /// <summary>
        /// The filter to use.
        /// </summary>
        protected Func<INotifyPropertyChanged, PropertyChangedEventArgs, bool> filter;
       
        /// <summary>
        /// Creates the filterable notify property changed observer class.
        /// </summary>
        /// <param name="owner">The owner class</param>
        /// <param name="filter">The filter</param>
        public FilterableNotifyPropertyChangedObserver(INotifyPropertyChanged owner,
            Func<INotifyPropertyChanged, PropertyChangedEventArgs, bool> filter)
            : base(owner)
        {
            this.filter = filter;
        }

        /// <summary>
        /// The method invoked on property changed to notify the subscribers.
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The PropertyChangedEventArgs</param>
        protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (filter != null)
            {
                if (filter(base.owner, e))
                    base.OnPropertyChanged(sender, e);
            }
            else
                base.OnPropertyChanged(sender, e);
        }
    }

}
