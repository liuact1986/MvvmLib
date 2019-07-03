using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The current source changed event args class.
    /// </summary>
    public class CurrentSourceChangedEventArgs : EventArgs
    {
        private readonly int currentIndex;
        /// <summary>
        /// Gets the current index;
        /// </summary>
        public int CurrentIndex
        {
            get { return currentIndex; }
        }

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
        /// <param name="currentIndex">The current index</param>
        /// <param name="current">The current source</param>
        public CurrentSourceChangedEventArgs(int currentIndex, object current)
        {
            this.currentIndex = currentIndex;
            this.current = current;
        }
    }
}
