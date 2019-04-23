using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class LoadedEventListener : IDisposable
    {
        private readonly FrameworkElement element;
        public FrameworkElement Element
        {
            get { return element; }
        }

        private Action<object, RoutedEventArgs> callback;

        public LoadedEventListener(FrameworkElement element)
        {
            this.element = element;
        }

        private void Element_Loaded(object sender, RoutedEventArgs e)
        {
            this.callback(sender, e);
        }

        public void Subscribe(Action<object, RoutedEventArgs> callback)
        {
            this.callback = callback;
            element.Loaded += Element_Loaded;
        }

        public void Unsubscribe()
        {
            callback = null;
            element.Loaded -= Element_Loaded;
        }

        public void Dispose()
        {
            callback = null;
        }
    }
}
