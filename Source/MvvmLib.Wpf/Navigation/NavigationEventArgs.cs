using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation event args class.
    /// </summary>
    public class NavigationEventArgs : EventArgs
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

        private readonly NavigationType navigationType;
        /// <summary>
        /// The navigation type.
        /// </summary>
        public NavigationType NavigationType
        {
            get { return navigationType; }
        }

        internal NavigationEventArgs(Type sourceType, object parameter, NavigationType navigationType)
        {
            this.sourceType = sourceType;
            this.parameter = parameter;
            this.navigationType = navigationType;
        }
    }
}