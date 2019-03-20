using System;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The frame navigating event class.
    /// </summary>
     public class FrameNavigatingEventArgs
    {

            /// <summary>
            /// The source page type.
            /// </summary>
            public Type SourcePageType { get; }

            /// <summary>
            /// The content of the page.
            /// </summary>
            public object Content { get; }

            /// <summary>
            /// The parameter.
            /// </summary>
            public object Parameter { get; }

        /// <summary>
        /// The navigation mode.
        /// </summary>
        public NavigationMode NavigationMode { get; }

        /// <summary>
        /// Creates the frame navigating event class.
        /// </summary>
        /// <param name="sourcePageType">The source page type</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="navigationMode">The navigation mode</param>
        public FrameNavigatingEventArgs(Type sourcePageType, object parameter, NavigationMode navigationMode)
        {
            this.SourcePageType = sourcePageType;
            this.Parameter = parameter;
            this.NavigationMode = navigationMode;
        }
    }
}