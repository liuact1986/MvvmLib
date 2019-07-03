using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigated event args class.
    /// </summary>
    public class NavigatedEventArgs : EventArgs
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

        private readonly object parameter;
        /// <summary>
        /// The parameter.
        /// </summary>
        public object Parameter
        {
            get { return parameter; }
        }


        private readonly NavigationType navigationType;
        /// <summary>
        /// The navigation type.
        /// </summary>
        public NavigationType NavigationType
        {
            get { return navigationType; }
        }

        internal NavigatedEventArgs(Type sourceType, object source, object parameter, NavigationType navigationType)
        {
            this.sourceType = sourceType;
            this.source = source;
            this.parameter = parameter;
            this.navigationType = navigationType;
        }
    }
}