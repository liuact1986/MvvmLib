using System;
using System.Collections.Generic;

namespace MvvmLib.Adaptive
{
    public class ActiveChangedEventArgs : EventArgs
    {
        public double Width { get; }
        public Dictionary<string, object> Active { get; }

        public ActiveChangedEventArgs(double width, Dictionary<string, object> active)
        {
            this.Width = width;
            this.Active = active;
        }
    }

}
