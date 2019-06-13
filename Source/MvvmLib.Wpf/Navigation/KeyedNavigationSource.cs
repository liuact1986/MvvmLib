namespace MvvmLib.Navigation
{

    /// <summary>
    /// <see cref="NavigationSource"/> with key.
    /// </summary>
    public class KeyedNavigationSource : NavigationSource
    {
        private readonly string key;
        /// <summary>
        /// The key.
        /// </summary>
        public string Key
        {
            get { return key; }
        }

        /// <summary>
        /// Creates the <see cref="KeyedNavigationSource"/>.
        /// </summary>
        /// <param name="key">The key</param>
        public KeyedNavigationSource(string key)
        {
            this.key = key;
        }
    }
}
