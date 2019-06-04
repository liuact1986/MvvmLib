namespace MvvmLib.Navigation
{


    /// <summary>
    /// Allows to be notified when the source is selected (selection changed event) for Selector (ListBox, TabControl, etc.).
    /// </summary>
    public interface IIsSelected
    {
        /// <summary>
        /// The property used to notify selection changed.
        /// </summary>
        bool IsSelected { get; set; }
    }

}
