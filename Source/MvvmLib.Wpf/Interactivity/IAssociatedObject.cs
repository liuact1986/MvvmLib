using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// Allows to provide a dependency object to the bahavior.
    /// </summary>
    public interface IAssociatedObject
    {
        /// <summary>
        /// The dependency object / control.
        /// </summary>
        DependencyObject AssociatedObject { get; set; }
    }
}
