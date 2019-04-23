using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Animation
{
    public class AnimatableContentControl : ContentControl
    {
        private const string PreviousContentPresenterPartName = "PreviousContentPresenter";
        private const string CurrentContentPresenterPartName = "CurrentContentPresenter";
        private Queue<NavigationQueueItem> navigationQueue;

        private ContentPresenter currentContentPresenter;
        private ContentPresenter previousContentPresenter;

        public object OldContent => currentContentPresenter.Content;

        private bool isAnimating;
        public bool IsAnimating
        {
            get { return isAnimating; }
        }

        public IContentAnimation EntranceAnimation
        {
            get { return (IContentAnimation)GetValue(EntranceAnimationProperty); }
            set { SetValue(EntranceAnimationProperty, value); }
        }

        public static readonly DependencyProperty EntranceAnimationProperty =
            DependencyProperty.Register("EntranceAnimation", typeof(IContentAnimation), typeof(AnimatableContentControl), new PropertyMetadata(null));

        public IContentAnimation ExitAnimation
        {
            get { return (IContentAnimation)GetValue(ExitAnimationProperty); }
            set { SetValue(ExitAnimationProperty, value); }
        }

        public static readonly DependencyProperty ExitAnimationProperty =
            DependencyProperty.Register("ExitAnimation", typeof(IContentAnimation), typeof(AnimatableContentControl), new PropertyMetadata(null));

        public bool Simultaneous
        {
            get { return (bool)GetValue(SimultaneousProperty); }
            set { SetValue(SimultaneousProperty, value); }
        }

        public static readonly DependencyProperty SimultaneousProperty =
            DependencyProperty.Register("Simultaneous", typeof(bool), typeof(AnimatableContentControl), new PropertyMetadata(false));


        public Action OnCompleted
        {
            get { return (Action)GetValue(OnCompletedProperty); }
            set { SetValue(OnCompletedProperty, value); }
        }

        public static readonly DependencyProperty OnCompletedProperty =
            DependencyProperty.Register("OnCompleted", typeof(Action), typeof(AnimatableContentControl), new PropertyMetadata(null));

        public Action OnCancelled
        {
            get { return (Action)GetValue(OnCancelledProperty); }
            set { SetValue(OnCancelledProperty, value); }
        }

        public static readonly DependencyProperty OnCancelledProperty =
            DependencyProperty.Register("OnCancelled", typeof(Action), typeof(AnimatableContentControl), new PropertyMetadata(null));

        static AnimatableContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnimatableContentControl), new FrameworkPropertyMetadata(typeof(AnimatableContentControl)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.navigationQueue = new Queue<NavigationQueueItem>();
            this.previousContentPresenter = this.GetTemplateChild(PreviousContentPresenterPartName) as ContentPresenter;
            this.currentContentPresenter = this.GetTemplateChild(CurrentContentPresenterPartName) as ContentPresenter;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            this.Run(newContent);
        }

        private void DoOnLeave(object oldContent, Action onLeaveCompleted)
        {
            if (ExitAnimation != null && oldContent is UIElement oldElement)
            {
                ExitAnimation.Run(oldElement,
                    () => onLeaveCompleted(),
                    () => OnCancelled?.Invoke());
            }
            else
                onLeaveCompleted();
        }

        private void DoOnEnter(object newContent, Action onEnterCompleted)
        {
            if (EntranceAnimation != null && newContent is UIElement newElement)
            {
                EntranceAnimation.Run(newElement,
                    () => onEnterCompleted(),
                    () => OnCancelled?.Invoke());
            }
            else
                onEnterCompleted();
        }

        private void Reset(object content)
        {
            if (content is UIElement element)
            {
                element.Opacity = 1;
                element.RenderTransform = null;
            }
        }

        private void DoNavigateInternal(object newContent, Action next)
        {
            isAnimating = true;

            var oldContent = OldContent;

            if (Simultaneous)
            {
                //Reset(oldContent);
                int i = 0;
                previousContentPresenter.Content = oldContent;
                previousContentPresenter.Visibility = Visibility.Visible;
                DoOnLeave(oldContent, () =>
                {
                    previousContentPresenter.Visibility = Visibility.Collapsed;
                    i++;
                    if (i == 2)
                        next();
                });

                //Reset(newContent);
                currentContentPresenter.Content = newContent;
                DoOnEnter(newContent, () =>
                {
                    i++;
                    if (i == 2)
                        next();
                });
            }
            else
            {
                //Reset(oldContent);
                previousContentPresenter.Content = oldContent;
                previousContentPresenter.Visibility = Visibility.Visible;
                DoOnLeave(oldContent, () =>
                {
                    //Reset(newContent);
                    previousContentPresenter.Visibility = Visibility.Collapsed;
                    currentContentPresenter.Content = newContent;
                    DoOnEnter(newContent, () => next());
                });
            }
        }

        private void DequeueNavigationInternal()
        {
            if (navigationQueue.Count > 0)
            {
                var navigationQueueItem = navigationQueue.Dequeue();
                DoNavigateInternal(navigationQueueItem.Content, navigationQueueItem.OnCompleted);
            }
            else
            {
                isAnimating = false;
                OnCompleted?.Invoke();
            }
        }


        public void Run(object newContent)
        {
            var action = new Action(() => DequeueNavigationInternal());

            if (isAnimating)
                navigationQueue.Enqueue(new NavigationQueueItem(newContent, action));
            else
                this.DoNavigateInternal(newContent, action);
        }

        public void CancelAnimations()
        {
            if (isAnimating)
            {
                navigationQueue.Clear();

                if (ExitAnimation != null)
                    ExitAnimation.CancelAnimations();
                if (EntranceAnimation != null)
                    EntranceAnimation.CancelAnimations();

                previousContentPresenter.Content = null;
                previousContentPresenter.Visibility = Visibility.Collapsed;
                currentContentPresenter.Content = null;

                isAnimating = false;
            }
        }
    }

}
