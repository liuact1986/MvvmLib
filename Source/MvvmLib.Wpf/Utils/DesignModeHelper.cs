using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Utils
{
    /// <summary>
    /// The design mode helper.
    /// </summary>
    public class DesignModeHelper
    {
        /// <summary>
        /// Checks if is in design mode.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>True if in design mode</returns>
        public static bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }
    }
}
