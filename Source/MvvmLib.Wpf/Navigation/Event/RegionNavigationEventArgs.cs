using System;

namespace MvvmLib.Navigation
{

    public class RegionNavigationEventArgs : EventArgs
    {
        public Type SourcePageType { get; }
        public object Parameter { get; }
        public RegionNavigationType RegionNavigationType { get; }

        public RegionNavigationEventArgs(Type sourcePageType, object parameter, RegionNavigationType regionNavigationType)
        {
            SourcePageType = sourcePageType;
            Parameter = parameter;
            RegionNavigationType = regionNavigationType;
        }
    }

}