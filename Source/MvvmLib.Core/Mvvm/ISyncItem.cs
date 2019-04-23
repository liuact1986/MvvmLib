namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Updates the values only if required.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISyncItem<T>
    {
        /// <summary>
        /// Synchronize the current to other value(s).
        /// </summary>
        /// <param name="other"></param>
        void Sync(T other);

        /// <summary>
        /// Checks if the update is required.
        /// </summary>
        /// <param name="other">The other value</param>
        /// <returns>Tru if required</returns>
        bool NeedSync(T other);
    }
}
