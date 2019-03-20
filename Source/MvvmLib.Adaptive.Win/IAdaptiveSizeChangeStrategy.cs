using System;

namespace MvvmLib.Adaptive
{
    public interface IAdaptiveSizeChangeStrategy
    {
        double CurrentWidth { get; }
        bool HasWidth { get; }

        event EventHandler<AdaptiveSizeChangedEventArgs> SizeChanged;
    }

}
