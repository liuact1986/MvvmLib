using System;
using Windows.UI.Xaml.Navigation;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The frame navigated event class.
    /// </summary>
    public class FrameNavigatedEventArgs
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
        /// Creates the frame navigated event class.
        /// </summary>
        /// <param name="sourcePageType">The source page type</param>
        /// <param name="content">The content of the page</param>
        /// <param name="parameter">The parameter</param>
        /// <param name="navigationMode">The navigation mode</param>
        public FrameNavigatedEventArgs(Type sourcePageType, object content, object parameter, NavigationMode navigationMode)
        {
            this.SourcePageType = sourcePageType;
            this.Content = content;
            this.Parameter = parameter;
            this.NavigationMode = navigationMode;
        }
    }
}