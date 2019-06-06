namespace MvvmLib.Mvvm
{
    /// <summary>
    /// The validation type.
    /// </summary>
    public enum ValidationHandling
    {
        /// <summary>
        /// On property changed
        /// </summary>
        OnPropertyChange,
        /// <summary>
        /// After ValidateAll invoked
        /// </summary>
        OnSubmit,
        /// <summary>
        /// Only with ValidateAll and ValidateProperty invoked
        /// </summary>
        Explicit
    }

}
