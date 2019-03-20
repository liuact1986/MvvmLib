using System;
using System.Collections.Generic;

namespace MvvmLib.Adaptive
{
    public interface IBreakpointListener
    {
        IReadOnlyList<double> Breakpoints { get; }
        double CurrentBreakpoint { get; }
        double CurrentWidth { get; }

        event EventHandler<BreakpointChangedEventArgs> BreakpointChanged;

        void AddBreakpoint(double width);
        bool HasSizeInf(double width);
    }
}