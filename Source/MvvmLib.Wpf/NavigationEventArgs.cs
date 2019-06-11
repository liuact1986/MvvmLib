using System;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation event args class.
    /// </summary>
    public class NavigationEventArgsBase : EventArgs
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="source"></param>
        /// <param name="parameter"></param>
        /// <param name="navigationType"></param>
        public NavigationEventArgsBase(Type sourceType, object parameter, NavigationType navigationType)
        {
            this.sourceType = sourceType;
            this.parameter = parameter;
            this.navigationType = navigationType;
        }
    }

    /// <summary>
    /// The navigating event args class.
    /// </summary>
    public class NavigatingEventArgs : NavigationEventArgsBase
    {
        public NavigatingEventArgs(Type sourceType, object parameter, NavigationType navigationType) : base(sourceType, parameter, navigationType)
        {
        }
    }

    /// <summary>
    /// The navigated event args class.
    /// </summary>
    public class NavigatedEventArgs : NavigationEventArgsBase
    {
        public NavigatedEventArgs(Type sourceType, object parameter, NavigationType navigationType) : base(sourceType, parameter, navigationType)
        {
        }
    }
}