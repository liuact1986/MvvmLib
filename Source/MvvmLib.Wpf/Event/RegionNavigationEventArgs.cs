using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The region navigation event args class.
    /// </summary>
    public class RegionNavigationEventArgs : EventArgs
    {
        private readonly Type sourceType;
        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
        }

        private readonly RegionNavigationType regionNavigationType;
        /// <summary>
        /// The navigation type.
        /// </summary>
        public RegionNavigationType RegionNavigationType
        {
            get { return regionNavigationType; }
        }

        /// <summary>
        /// Creates the region navigation event args class.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="regionNavigationType">The navigation type</param>
        public RegionNavigationEventArgs(Type sourceType, object parameter, RegionNavigationType regionNavigationType)
        {
            this.sourceType = sourceType;
            this.parameter = parameter;
            this.regionNavigationType = regionNavigationType;
        }
    }

}