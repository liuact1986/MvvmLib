using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The current source changed event args class.
    /// </summary>
    public class CurrentSourceChangedEventArgs : EventArgs
    {
        private readonly object current;
        /// <summary>
        /// Gets the current source.
        /// </summary>
        public object Current
        {
            get { return current; }
        }

        /// <summary>
        /// Creates the <see cref="CurrentSourceChangedEventArgs"/>.
        /// </summary>
        /// <param name="current">The current source</param>
        public CurrentSourceChangedEventArgs(object current)
        {
            this.current = current;
        }
    }
}
