using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The shared source selected item changed event args.
    /// </summary>
    public class SharedSourceSelectedItemChangedEventArgs : EventArgs
    {
        private readonly int selectedIndex;
        /// <summary>
        /// The new selected index.
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
        }

        private readonly object selectedItem;
        /// <summary>
        /// The new selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return selectedItem; }
        }

        /// <summary>
        /// Creates the <see cref="SharedSourceSelectedItemChangedEventArgs"/>.
        /// </summary>
        /// <param name="selectedIndex">The new selectedIndex</param>
        /// <param name="selectedItem">The new selected item.</param>
        public SharedSourceSelectedItemChangedEventArgs(int selectedIndex, object selectedItem)
        {
            this.selectedIndex = selectedIndex;
            this.selectedItem = selectedItem;
        }
    }

}
