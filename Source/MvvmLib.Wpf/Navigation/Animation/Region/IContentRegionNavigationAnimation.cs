using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public interface IContentRegionNavigationAnimation
    {
        ContentPresenter CurrentPresenter { get; }
        IContentAnimation EntranceAnimation { get; set; }
        IContentAnimation ExitAnimation { get; set; }
        bool IsAnimating { get; }
        object OldContent { get; }
        ContentPresenter PreviousPresenter { get; }
        bool Simultaneous { get; set; }
        Grid ViewContainer { get; }

        void Reset(object content);
        void Start(object newContent);
    }
}