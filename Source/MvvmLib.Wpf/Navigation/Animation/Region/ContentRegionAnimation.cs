using System;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Navigation
{
    public class ContentRegionAnimation : IContentRegionAnimation
    {
        private Grid viewContainer;
        public Grid ViewContainer
        {
            get { return viewContainer; }
            protected set { viewContainer = value; }
        }

        private ContentPresenter previousPresenter;
        public ContentPresenter PreviousPresenter
        {
            get { return previousPresenter; }
            protected set { previousPresenter = value; }
        }

        private ContentPresenter currentPresenter;
        public ContentPresenter CurrentPresenter
        {
            get { return currentPresenter; }
            protected set { currentPresenter = value; }
        }

        public object OldContent => CurrentPresenter.Content;

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

        private bool simultaneous;
        public bool Simultaneous
        {
            get { return simultaneous; }
            set { simultaneous = value; }
        }

        public ContentRegionAnimation(ContentControl control)
        {
            ViewContainer = new Grid();
            PreviousPresenter = new ContentPresenter();
            CurrentPresenter = new ContentPresenter();
            ViewContainer.Children.Add(PreviousPresenter);
            ViewContainer.Children.Add(CurrentPresenter);

            control.Content = ViewContainer;
        }

        protected void DoOnLeave(object oldContent, Action onLeaveCompleted)
        {
            if (oldContent != null && oldContent is UIElement element)
            {
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

        protected void DoOnEnter(object newContent, Action onEnterCompleted)
        {
            if (newContent != null && newContent is UIElement element)
            {
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

        public void Reset(object content)
        {
            if (content != null && content is UIElement element)
            {
                element.Opacity = 1;
                element.RenderTransform = null;
            }
        }

        public void Start(object newContent)
        {
            var oldContent = OldContent;
            if (Simultaneous)
            {
                Reset(oldContent);

                PreviousPresenter.Content = oldContent;
                PreviousPresenter.Visibility = Visibility.Visible;
                DoOnLeave(oldContent, () =>
                {
                    PreviousPresenter.Visibility = Visibility.Collapsed;
                });

                Reset(newContent);

                CurrentPresenter.Content = newContent;
                DoOnEnter(newContent, () =>
                {

                });
            }
            else
            {
                Reset(oldContent);

                PreviousPresenter.Content = oldContent;
                PreviousPresenter.Visibility = Visibility.Visible;
                DoOnLeave(oldContent, () =>
                {
                    Reset(newContent);

                    PreviousPresenter.Visibility = Visibility.Collapsed;
                    CurrentPresenter.Content = newContent;
                    DoOnEnter(newContent, () =>
                    {

                    });
                });
            }
        }
    }

}
