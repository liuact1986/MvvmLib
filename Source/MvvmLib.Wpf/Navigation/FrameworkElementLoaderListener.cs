using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class FrameworkElementLoaderListener : IDisposable
    {
        public FrameworkElement Element { get; }

        Action<object, RoutedEventArgs> callback;

        public FrameworkElementLoaderListener(FrameworkElement element)
        {
            this.Element = element;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            this.callback(sender, e);
        }

        public void Subscribe(Action<object, RoutedEventArgs> callback)
        {
            this.callback = callback;
            Element.Loaded += Element_Loaded;
        }

        public void Unsubscribe()
        {
            callback = null;
            Element.Loaded -= Element_Loaded;
        }

        public void Dispose()
        {
            callback = null;
        }
    }
}
