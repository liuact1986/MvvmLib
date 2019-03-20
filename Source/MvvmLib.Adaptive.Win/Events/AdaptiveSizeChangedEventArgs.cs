using System;

namespace MvvmLib.Adaptive
{
    public class AdaptiveSizeChangedEventArgs : EventArgs
    {
        public double Width { get; }

        public AdaptiveSizeChangedEventArgs(double width)
        {
            this.Width = width;
        }
    }

}
