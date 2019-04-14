using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class NavigationEntry
    {
        public Type SourceType { get; }
        public object ViewOrObject { get;  }
        public object Parameter { get; }
        public object Context { get; }

        public List<RegionBase> ChildRegions { get; internal set; }

        public NavigationEntry(Type viewType, object view, object parameter, object context)
        {
            ChildRegions = new List<RegionBase>();
            this.SourceType = viewType;
            this.ViewOrObject = view;
            this.Parameter = parameter;
            this.Context = context;
        }
    }
}