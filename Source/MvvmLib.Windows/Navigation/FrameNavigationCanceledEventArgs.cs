using System;

namespace MvvmLib.Navigation
{

    /// <summary>
    /// The frame navigation failed event class.
    /// </summary>
    public class FrameNavigationCanceledEventArgs : EventArgs
    {

        /// <summary>
        /// The source of cancelation.
        /// </summary>
        public object Source { get; }


        /// <summary>
        /// Creates the navigation failed event class.
        /// </summary>
        /// <param name="source">The source of cancelation</param>
        public FrameNavigationCanceledEventArgs(object source)
        {
            this.Source = source;
        }
    }
}