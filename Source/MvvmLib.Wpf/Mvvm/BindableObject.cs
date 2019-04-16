using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Core.Mvvm
{
    /// <summary>
    /// Allows to bind a value or object to Value dependency property and be notified on value changed.
    /// </summary>
    /// <typeparam name="T">The type of the value to observe</typeparam>
    public class BindableObject<T> : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// The property changed to subscribe to be notified on value changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The Value.
        /// </summary>
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// The value property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(T), typeof(BindableObject<T>), new PropertyMetadata(OnValueChanged));


        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableObject = (BindableObject<T>)d;
            if (bindableObject != null)
                bindableObject.PropertyChanged?.Invoke(bindableObject, new PropertyChangedEventArgs("Value"));
        }
    }
}
