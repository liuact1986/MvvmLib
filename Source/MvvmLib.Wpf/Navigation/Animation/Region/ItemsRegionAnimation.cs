using System;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class ItemsRegionAnimation : IItemsRegionAnimation
    {
        private IContentAnimation entranceAnimation;
        public IContentAnimation EntranceAnimation
        {
            get { return entranceAnimation; }
            set { entranceAnimation = value; }
        }

        private IContentAnimation exitAnimation;
        public IContentAnimation ExitAnimation
        {
            get { return exitAnimation; }
            set { exitAnimation = value; }
        }

        public void DoOnLeave(object oldContent, Action onLeaveCompleted)
        {
            if (oldContent != null && oldContent is UIElement element)
            {
                Reset(element);
                if (ExitAnimation != null)
                    ExitAnimation.Start(element, () =>
                    {
                        onLeaveCompleted();
                    });
                else
                    onLeaveCompleted();
            }
            else
                onLeaveCompleted();
        }

        public void DoOnEnter(object newContent, Action onEnterCompleted)
        {
            if (newContent != null && newContent is UIElement element)
            {
                //Reset(element);
                if (EntranceAnimation != null)
                    EntranceAnimation.Start(element, () =>
                    {
                        onEnterCompleted();
                    });
                else
                    onEnterCompleted();
            }
            else
                onEnterCompleted();
        }

        public void Reset(UIElement element)
        {
            element.Opacity = 1;
            element.RenderTransform = null;
        }
    }

}
