using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to provide an associated object.
    /// </summary>
    public interface IAssociatedObject
    {
        /// <summary>
        /// The associated object.
        /// </summary>
        DependencyObject AssociatedObject { get; }
    }
}
