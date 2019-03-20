using System;

namespace MvvmLib.Navigation
{
    public interface ISelectable
    {
        bool IsTarget(Type viewType, object parameter);
    }
}