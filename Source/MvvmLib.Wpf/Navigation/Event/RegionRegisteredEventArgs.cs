using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region registration event args class.
    /// </summary>
    public class RegionRegisteredEventArgs : EventArgs
    {
        private string regionName;

        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }

        private IRegion region;
        /// <summary>
        /// The region created.
        /// </summary>
        public IRegion Region
        {
            get { return region; }
        }

        /// <summary>
        /// Creates the region registration event args class.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="region">The region created</param>
        public RegionRegisteredEventArgs(string regionName, IRegion region)
        {
            this.regionName = regionName;
            this.region = region;
        }
    }
}
