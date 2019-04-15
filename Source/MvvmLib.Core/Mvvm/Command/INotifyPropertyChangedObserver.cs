using System;
using System.ComponentModel;

namespace MvvmLib.Mvvm
{
    /// <summary>
    /// Allows to subscribe and notify on property changed for an object that implements <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    public interface INotifyPropertyChangedObserver
    {
        /// <summary>
        /// Subscribe to property changed of the owner.
        /// </summary>
        /// <param name="onPropertyChangedCallback">The callback invoked on property changed</param>
        void SubscribeToPropertyChanged(Action<PropertyChangedEventArgs> onPropertyChangedCallback);

        /// <summary>
        /// Unsubscribe to property changed of the owner.
        /// </summary>
        void UnsubscribeToPropertyChanged();
    }

}
