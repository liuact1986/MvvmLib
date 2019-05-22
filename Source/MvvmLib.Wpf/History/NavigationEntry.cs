using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation entry class.
    /// </summary>
    public class NavigationEntry 
    {
        private readonly Type sourceType;
        /// <summary>
        /// The source type.
        /// </summary>
        public Type SourceType
        {
            get { return sourceType; }
        }

        private readonly object source;
        /// <summary>
        /// The view or object.
        /// </summary>
        public object Source
        {
            get { return source; }
        }

        private readonly object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
        }

        private readonly object context;
        /// <summary>
        /// The context.
        /// </summary>
        public object Context
        {
            get { return context; }
        }

        internal List<RegionBase> childRegions;
        /// <summary>
        /// The child regions.
        /// </summary>
        public List<RegionBase> ChildRegions
        {
            get { return childRegions; }
        }

        /// <summary>
        /// Creates the navigation entry.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="viewOrObject">The view or object</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="context">The context</param>
        public NavigationEntry(Type sourceType, object viewOrObject, object parameter, object context)
        {
            this.childRegions = new List<RegionBase>();
            this.sourceType = sourceType;
            this.source = viewOrObject;
            this.parameter = parameter;
            this.context = context;
        }
    }
}
