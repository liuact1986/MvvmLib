using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The items region adapter contract.
    /// </summary>
    public interface IItemsRegionAdapter
    {
        /// <summary>
        /// The control.
        /// </summary>
        DependencyObject Control { get; set; }

        /// <summary>
        /// Allows to bind control to region.
        /// </summary>
        /// <param name="region">The items region</param>
        void Adapt(ItemsRegion region);
    }
}