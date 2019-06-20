using System;

namespace MvvmLib.History
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
        /// The source.
        /// </summary>
        public object Source
        {
            get { return source; }
        }

        private object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
            internal set { parameter = value; }
        }

        /// <summary>
        /// Creates the navigation entry.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="source">The source</param>
        /// <param name="parameter">The parameter</param>
        public NavigationEntry(Type sourceType, object source, object parameter)
        {
            this.sourceType = sourceType;
            this.source = source;
            this.parameter = parameter;
        }
    }
}
