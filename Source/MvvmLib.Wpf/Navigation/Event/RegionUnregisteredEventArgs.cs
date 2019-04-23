using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region unregistration event args class.
    /// </summary>
    public class RegionUnregisteredEventArgs : EventArgs
    {
        private string regionName;

        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }

        /// <summary>
        /// Creates the region unregistration event args class.
        /// </summary>
        /// <param name="regionName">The region name</param>
        public RegionUnregisteredEventArgs(string regionName)
        {
            this.regionName = regionName;
        }
    }
}
