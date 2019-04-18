using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public interface IContentRegionAnimation
    {
        ContentPresenter CurrentPresenter { get; }
        IContentAnimation EntranceAnimation { get; set; }
        IContentAnimation ExitAnimation { get; set; }
        object OldContent { get; }
        ContentPresenter PreviousPresenter { get; }
        bool Simultaneous { get; set; }
        Grid ViewContainer { get; }

        void Start(object newContent);
    }
}