using System;
using System.Collections.Generic;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class SelectableResolver
    {
        public int TrySelect(Type viewType, object parameter, List<object> viewOrObjects)
        {
            for (int i = 0; i < viewOrObjects.Count; i++)
            {
                var view = viewOrObjects[i] as FrameworkElement;
                if (view != null && view.DataContext is ISelectable)
                {
                    if (((ISelectable)view.DataContext).IsTarget(viewType, parameter))
                    {
                        if (!view.Focus())
                            if (view.Parent is UIElement)
                                ((UIElement)view.Parent).Focus();


                        return i;
                    }
                }
            }
            return -1;
        }

    }

}