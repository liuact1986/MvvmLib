using System;

namespace MvvmLib.Navigation
{
    public interface IContentRegionAdapter
    {
        Type TargetType { get; }

        object GetContent(object control);
        void OnGoBack(object control, object previousView);
        void OnGoForward(object control, object nextView);
        void OnNavigate(object control, object view);
    }
}