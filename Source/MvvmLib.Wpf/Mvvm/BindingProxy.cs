using System.Windows;

namespace MvvmLib.Wpf.Mvvm
{

    /// <summary>
    /// A proxy for Binding. Usefull with DataGrid columns for example.
    /// </summary>
    public class BindingProxy : Freezable
    {
        /// <summary>
        /// Creates the <see cref="BindingProxy"/>.
        /// </summary>
        /// <returns>The binding proxy instance</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        /// <summary>
        /// The DataContext.
        /// </summary>
        public object Context
        {
            get { return (object)GetValue(ContextProperty); }
            set { SetValue(ContextProperty, value); }
        }

        /// <summary>
        /// The DataContext.
        /// </summary>
        public static readonly DependencyProperty ContextProperty =
            DependencyProperty.Register("Context", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
