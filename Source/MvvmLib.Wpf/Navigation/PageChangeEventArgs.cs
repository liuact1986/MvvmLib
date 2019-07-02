using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The page change event args class.
    /// </summary>
    public class PageChangeEventArgs : CancelEventArgs
    {

        private readonly int newPageIndex;
        /// <summary>
        /// The new page index.
        /// </summary>
        public int NewPageIndex
        {
            get { return newPageIndex; }
        }

        /// <summary>
        /// Creates the <see cref="PageChangeEventArgs"/>.
        /// </summary>
        /// <param name="newPageIndex">The new page index</param>
        public PageChangeEventArgs(int newPageIndex)
        {
            this.newPageIndex = newPageIndex;
        }
    }

}
