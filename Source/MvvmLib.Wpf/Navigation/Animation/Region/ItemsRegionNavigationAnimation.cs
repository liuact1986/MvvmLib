using System;
using System.Collections.Generic;
using System.Windows;

namespace MvvmLib.Navigation
{
    public class ItemsRegionNavigationAnimation : IItemsRegionNavigationAnimation
    {
        private readonly FrameworkElement control;
        public FrameworkElement Control
        {
            get { return control; }
        }

        private readonly IItemsRegionAdapter itemsRegionAdapter;
        public IItemsRegionAdapter ItemsRegionAdapter
        {
            get { return itemsRegionAdapter; }
        }

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

        public bool IsAnimating
        {
            get
            {
                var isAnimating = (entranceAnimation != null && entranceAnimation.IsAnimating)
                    || (exitAnimation != null && exitAnimation.IsAnimating);
                return isAnimating;
            }
        }

        private bool isEntering;
        public bool IsEntering
        {
            get { return isEntering; }
        }

        private bool isLeaving;
        public bool IsLeaving
        {
            get { return isLeaving; }
        }

        private Queue<NavigationQueueItem> onEnterQueue;
        public Queue<NavigationQueueItem> OnEnterQueue
        {
            get { return onEnterQueue; }
        }

        private Queue<NavigationQueueItem> onLeaveQueue;
        public Queue<NavigationQueueItem> OnLeaveQueue
        {
            get { return onLeaveQueue; }
        }

        public ItemsRegionNavigationAnimation(FrameworkElement control, IItemsRegionAdapter itemsRegionAdapter)
        {
            onEnterQueue = new Queue<NavigationQueueItem>();
            onLeaveQueue = new Queue<NavigationQueueItem>();
            this.control = control;
            this.itemsRegionAdapter = itemsRegionAdapter;
        }

        private void DequeueOnEnterInternal()
        {
            if (onEnterQueue.Count > 0)
            {
                var navigationQueueItem = onEnterQueue.Dequeue();
                DoOnEnterInternal(navigationQueueItem.Content, navigationQueueItem.Index, navigationQueueItem.OnCompleted);
            }
            else
            {
                isEntering = false;
            }
        }

        private void DoOnEnterInternal(object newContent, int index, Action onCompleted)
        {
            ItemsRegionAdapter.OnInsert(Control, newContent, index);
            if (newContent != null && newContent is UIElement element)
            {
                //Reset(element);
                if (EntranceAnimation != null)
                    EntranceAnimation.Start(element, () =>
                    {
                        onCompleted();
                    });
                else
                    onCompleted();
            }
            else
                onCompleted();
        }

        public void DoOnEnter(object newContent, int index, Action onEnterCompleted)
        {
            isEntering = true;

            var action = new Action(() =>
            {
                //onEnterCompleted();
                DequeueOnEnterInternal();
            });

            if (entranceAnimation != null && entranceAnimation.IsAnimating)
                onEnterQueue.Enqueue(new NavigationQueueItem(newContent, index, action));
            else
                this.DoOnEnterInternal(newContent, index, action);
        }

        private void DequeueOnLeaveInternal()
        {
            if (onLeaveQueue.Count > 0)
            {
                var navigationQueueItem = onLeaveQueue.Dequeue();
                DoOnLeaveInternal(navigationQueueItem.Content, navigationQueueItem.Index, navigationQueueItem.OnCompleted);
            }
            else
            {
                isLeaving = false;
            }
        }

        private void DoOnLeaveInternal(object oldContent, int index, Action onCompleted)
        {
            if (oldContent != null && oldContent is UIElement element)
            {
                Reset(element);
                if (ExitAnimation != null)
                    ExitAnimation.Start(element, () =>
                    {
                        onCompleted();
                    });
                else
                    onCompleted();
            }
            else
                onCompleted();
        }

        public void DoOnLeave(object oldContent, int index, Action onEnterCompleted)
        {
            isLeaving = true;

            var action = new Action(() =>
            {
                ItemsRegionAdapter.OnRemoveAt(Control, index);
                //onEnterCompleted();
                DequeueOnLeaveInternal();
            });

            if (exitAnimation != null && exitAnimation.IsAnimating)
                onLeaveQueue.Enqueue(new NavigationQueueItem(oldContent, index, action));
            else
                this.DoOnLeaveInternal(oldContent, index, action);
        }

        public void Reset(UIElement element)
        {
            element.Opacity = 1;
            element.RenderTransform = null;
        }
    }

    public class NavigationQueueItem
    {
        private object content;
        public object Content
        {
            get { return content; }
            set { content = value; }
        }

        private Action onCompleted;
        public Action OnCompleted
        {
            get { return onCompleted; }
            set { onCompleted = value; }
        }

        private int index;
        public int Index { get => index; set => index = value; }

        public NavigationQueueItem(object content, int index, Action onCompleted)
        {
            Content = content;
            this.Index = index;
            OnCompleted = onCompleted;
        }
    }

}
