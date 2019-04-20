using System;
using System.Collections.Generic;

namespace MvvmLib.Navigation
{
    public class NavigationEntry
    {
        private readonly Type sourceType;
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object viewOrObject;
        public object ViewOrObject
        {
            get { return viewOrObject; }
        }

        private readonly object parameter;
        public object Parameter
        {
            get { return parameter; }
        }

        private readonly object context;
        public object Context
        {
            get { return context; }
        }

        private List<RegionBase> childRegions;
        public List<RegionBase> ChildRegions
        {
            get { return childRegions; }
            internal set { childRegions = value; }
        }

        public NavigationEntry(Type viewType, object view, object parameter, object context)
        {
            this.childRegions = new List<RegionBase>();
            this.sourceType = viewType;
            this.viewOrObject = view;
            this.parameter = parameter;
            this.context = context;
        }
    }
}