using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The view loaded event args class.
    /// </summary>
    public class ViewLoadedEventArgs : EventArgs
    {
        private string regionName;

        /// <summary>
        /// The region name.
        /// </summary>
        public string RegionName
        {
            get { return regionName; }
        }

        private object view;
        /// <summary>
        /// The view.
        /// </summary>
        public object View
        {
            get { return view; }
        }

        private IRegion region;
        /// <summary>
        /// The region.
        /// </summary>
        public IRegion Region
        {
            get { return region; }
        }

        /// <summary>
        /// Creates the view loaded event args class.
        /// </summary>
        /// <param name="regionName">The region name</param>
        /// <param name="view"></param>
        /// <param name="region">The region created</param>
        public ViewLoadedEventArgs(string regionName, object view, IRegion region)
        {
            this.regionName = regionName;
            this.view = view;
            this.region = region;
        }
    }
}
