using System;

namespace MvvmLib.Adaptive
{
    public class BreakpointChangedEventArgs : EventArgs
    {
        public double Width { get; }

        public BreakpointChangedEventArgs(double width)
        {
            this.Width = width;
        }
    }

}
