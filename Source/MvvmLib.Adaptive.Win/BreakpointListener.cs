using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MvvmLib.Adaptive
{
    public class DeferredActiveWidth
    {
        public double Width { get; set; }

        public DeferredActiveWidth(double width)
        {
            this.Width = width;
        }
    }

    /// <summary>
    /// Allows to define a list of breakpoints and be notified when the current breakpoint changed
    /// </summary>
    public class BreakpointListener : IBreakpointListener
    {
        protected IAdaptiveSizeChangeStrategy sizeChangeStrategy;

        // sorted list
        protected List<double> breakpoints;

        protected DeferredActiveWidth deferred;

        public IReadOnlyList<double> Breakpoints => this.breakpoints;

        public double CurrentWidth => this.sizeChangeStrategy.CurrentWidth;

        public double CurrentBreakpoint { get; protected set; } = -1;

        protected readonly List<EventHandler<BreakpointChangedEventArgs>> breakpointChanged;
        public event EventHandler<BreakpointChangedEventArgs> BreakpointChanged
        {
            add
            {
                if (!breakpointChanged.Contains(value))
                {
                    breakpointChanged.Add(value);
                }

                if (deferred != null)
                {
                    this.RaiseActiveChanged(deferred.Width);
                }
            }
            remove { if (breakpointChanged.Contains(value)) breakpointChanged.Remove(value); }
        }

        public BreakpointListener() : this(new MainWindowSizeChangeStrategy())
        { }

        public BreakpointListener(IAdaptiveSizeChangeStrategy sizeChangeStrategy)
        {
            this.breakpoints = new List<double>();
            this.breakpointChanged = new List<EventHandler<BreakpointChangedEventArgs>>();

            this.sizeChangeStrategy = sizeChangeStrategy;
            this.sizeChangeStrategy.SizeChanged += OnSizeChanged;
        }

        public bool HasSizeInf(double width)
        {
            foreach (var breakpoint in this.breakpoints)
            {
                if (breakpoint < width)
                {
                    return true;
                }
            }
            return false;
        }

        protected void TrySetCurrentBreakpoint(double width)
        {
            // 0 => 320 => 640 => 960 => 1280 => 1600 => ...
            var result = AdaptiveHelper.GuessActiveWidth(this.CurrentWidth, breakpoints);
            if (result != -1 && this.CurrentBreakpoint != result)
            {
                this.CurrentBreakpoint = result;
                // notify
                this.RaiseActiveChanged(this.CurrentBreakpoint);
            }
        }

        private void OnSizeChanged(object sender, AdaptiveSizeChangedEventArgs e)
        {
            this.TrySetCurrentBreakpoint(e.Width);
        }

        public void AddBreakpoint(double width)
        {
            if (!this.breakpoints.Contains(width))
            {
                this.breakpoints.Add(width);
                this.breakpoints = this.breakpoints.OrderBy(n => n).ToList();
                // deferred
                if (this.sizeChangeStrategy.HasWidth && this.HasSizeInf(this.sizeChangeStrategy.CurrentWidth))
                {
                    this.TrySetCurrentBreakpoint(this.CurrentWidth);
                }
            }
            else
            {
                Debug.WriteLine($"Caution the width {width} is defined multiple times");
            }
        }

        protected void RaiseActiveChanged(double width)
        {
            var context = new BreakpointChangedEventArgs(width);
            if (this.breakpointChanged.Count > 0)
            {
                foreach (var handler in breakpointChanged)
                {
                    handler(this, context);
                }
                deferred = null;
            }
            else
            {
                this.deferred = new DeferredActiveWidth(width);
            }
        }
    }

}
