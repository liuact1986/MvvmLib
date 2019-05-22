using MvvmLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace MvvmLib.Animation
{
    /// <summary>
    /// A control that allows to animate items on enter and leave.
    /// </summary>
    public class TransitioningItemsControl : Control
    {
        private const string InnerItemsControlPartName = "PART_Items";
        private ItemsControl innerItemsControl;
        private readonly Queue<TransitionQueueItem> actions;
        private bool canAnimate = true;

        /// <summary>
        /// The collection of items of the control.
        /// </summary>
        public ReadOnlyCollection Items
        {
            get { return new ReadOnlyCollection(innerItemsControl.Items); }
        }

        /// <summary>
        /// The entrance animation.
        /// </summary>
        public ParallelAnimation EntranceAnimation
        {
            get { return (ParallelAnimation)GetValue(EntranceAnimationProperty); }
            set { SetValue(EntranceAnimationProperty, value); }
        }

        /// <summary>
        /// The entrance animation.
        /// </summary>
        public static readonly DependencyProperty EntranceAnimationProperty =
            DependencyProperty.Register("EntranceAnimation", typeof(ParallelAnimation), typeof(TransitioningItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// The exit animation.
        /// </summary>
        public ParallelAnimation ExitAnimation
        {
            get { return (ParallelAnimation)GetValue(ExitAnimationProperty); }
            set { SetValue(ExitAnimationProperty, value); }
        }

        /// <summary>
        /// The exit animation.
        /// </summary>
        public static readonly DependencyProperty ExitAnimationProperty =
            DependencyProperty.Register("ExitAnimation", typeof(ParallelAnimation), typeof(TransitioningItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// The items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// The items source.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(TransitioningItemsControl),
                new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningItemsControl = d as TransitioningItemsControl;
            var source = e.NewValue as IEnumerable;
            if (source != null)
                transitioningItemsControl.SetItemsSource(source);
        }

        /// <summary>
        /// Allows to play or not animation on clear.
        /// </summary>
        public TransitionClearHandling TransitionClearHandling
        {
            get { return (TransitionClearHandling)GetValue(TransitionClearHandlingProperty); }
            set { SetValue(TransitionClearHandlingProperty, value); }
        }

        /// <summary>
        /// Allows to play or not animation on clear.
        /// </summary>
        public static readonly DependencyProperty TransitionClearHandlingProperty =
            DependencyProperty.Register("TransitionClearHandling", typeof(TransitionClearHandling), typeof(TransitioningItemsControl), new PropertyMetadata(TransitionClearHandling.Transition));

        /// <summary>
        /// The item template.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// The item template.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(TransitioningItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// The item template selector.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
            set { SetValue(ItemTemplateSelectorProperty, value); }
        }

        /// <summary>
        /// The item template selector.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(TransitioningItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// The item container style.
        /// </summary>
        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        /// <summary>
        /// The item container style.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(TransitioningItemsControl), new PropertyMetadata(null));

        /// <summary>
        /// The items panel. StackPanel by default.
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
            set { SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// The items panel. StackPanel by default.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(TransitioningItemsControl), new PropertyMetadata(GetDefaultItemsPanelTemplate()));

        private static ItemsPanelTemplate GetDefaultItemsPanelTemplate()
        {
            ItemsPanelTemplate template = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(StackPanel)));
            template.Seal();
            return template;
        }

        /// <summary>
        /// Allows to cancel the animation queue.
        /// </summary>
        public bool IsCancelled
        {
            get { return (bool)GetValue(IsCancelledProperty); }
            set { SetValue(IsCancelledProperty, value); }
        }

        /// <summary>
        /// Allows to cancel the animation queue.
        /// </summary>
        public static readonly DependencyProperty IsCancelledProperty =
            DependencyProperty.Register("IsCancelled", typeof(bool), typeof(TransitioningItemsControl), new PropertyMetadata(false, OnCancelledChanged));

        private static void OnCancelledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var transitioningItemsControl = d as TransitioningItemsControl;
            transitioningItemsControl.CancelTransition();
        }

        /// <summary>
        /// Checks if the control is animating.
        /// </summary>
        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            private set { SetValue(IsAnimatingProperty, value); }
        }

        /// <summary>
        /// Checks if the control is animating.
        /// </summary>
        private static readonly DependencyProperty IsAnimatingProperty =
            DependencyProperty.Register("IsAnimating", typeof(bool), typeof(TransitioningItemsControl), new PropertyMetadata(false));

        /// <summary>
        /// Invoked on transition completed.
        /// </summary>
        public event EventHandler TransitionCompleted;

        /// <summary>
        /// Invoked on transition cancelled.
        /// </summary>
        public event EventHandler TransitionCancelled;

        static TransitioningItemsControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TransitioningItemsControl), new FrameworkPropertyMetadata(typeof(TransitioningItemsControl)));
        }

        /// <summary>
        /// Creates the transitioning items control.
        /// </summary>
        public TransitioningItemsControl()
        {
            this.actions = new Queue<TransitionQueueItem>();
        }

        /// <summary>
        /// Apply template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.innerItemsControl = this.GetTemplateChild(InnerItemsControlPartName) as ItemsControl;
        }

        private void OnTransitionCompleted()
        {
            TransitionCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void OnTransitionCancelled()
        {
            TransitionCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void SetItemsSource(IEnumerable source)
        {
            if (source is INotifyCollectionChanged notifyCollectionChanged)
                notifyCollectionChanged.CollectionChanged += OnSourceCollectionChanged;
            else
                throw new ArgumentException("A collection that implements INotifyCollectionChanged is required for ItemsSource");
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            int index = -1;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        InsertItemOrEnqueue(index, item);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    index = e.OldStartingIndex;
                    foreach (var item in e.OldItems)
                    {
                        RemoveItemOrEnqueue(index);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    index = e.NewStartingIndex;
                    foreach (var item in e.NewItems)
                    {
                        SetItemOrEnqueue(index, item);
                        index++;
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotSupportedException("Move is not supported by the TransitioningItemsControl");
                case NotifyCollectionChangedAction.Reset:
                    ClearItemsOrEnqueue();
                    break;
            }
        }

        private void InsertItem(int index, object item)
        {
            innerItemsControl.Items.Insert(index, item);
        }

        private void SetItem(int index, object item)
        {
            innerItemsControl.Items[index] = item;
        }

        private void RemoveItem(int index)
        {
            innerItemsControl.Items.RemoveAt(index);
        }

        /// <summary>
        /// Allows to clear items without animations.
        /// </summary>
        public void ClearWithNoAnimation()
        {
            innerItemsControl.Items.Clear();
        }

        private void InsertAndAnimate(int index, object item)
        {
            // insert
            InsertItem(index, item);
            if (canAnimate && EntranceAnimation != null && item != null)
            {
                var newContent = XamlHelper.FindChild(item.GetType(), innerItemsControl, index);
                if (newContent == null)
                {
                    // try to find ContentPresenter for item that is ViewModel
                    newContent = XamlHelper.FindChild<ContentPresenter>(innerItemsControl, index);
                }

                if (newContent is FrameworkElement newElement)
                {
                    EntranceAnimation.BeginAnimation(newElement, () =>
                    {
                        DequeueActionInternal();
                    });
                }
                else
                {
                    // TODO: throw/log ?
                    DequeueActionInternal();
                }
            }
            else
            {
                DequeueActionInternal();
            }
        }

        private void RemoveAndAnimate(int index)
        {
            if (canAnimate && ExitAnimation != null)
            {
                var innerItemsControlItem = innerItemsControl.Items[index];
                var oldContent = XamlHelper.FindChild<ContentPresenter>(innerItemsControl, index);
                if (oldContent is FrameworkElement oldElement)
                {
                    ExitAnimation.BeginAnimation(oldElement, () =>
                    {
                        // remove
                        RemoveItem(index);
                        DequeueActionInternal();
                    });
                }
                else
                {
                    // TODO: throw/log ?
                    RemoveItem(index);
                    DequeueActionInternal();
                }
            }
            else
            {
                RemoveItem(index);
                DequeueActionInternal();
            }
        }

        /// <summary>
        /// Clears all items.
        /// </summary>
        public void ClearItemsOrEnqueue()
        {
            if (canAnimate && TransitionClearHandling == TransitionClearHandling.Transition)
            {
                int count = innerItemsControl.Items.Count;
                // 3 ... 2 ... 1 ... 0 
                for (int index = count - 1; index >= 0; index--)
                    RemoveItemOrEnqueue(index);
            }
            else
                this.ClearWithNoAnimation();
        }

        /// <summary>
        /// Sets the item or enque.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        public void SetItemOrEnqueue(int index, object item)
        {
            if (IsAnimating)
            {
                actions.Enqueue(new TransitionQueueItem(TransitionQueueItemActionType.Update, index, item));
            }
            else
                SetItem(index, item);
        }

        /// <summary>
        /// Removes the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        public void RemoveItemOrEnqueue(int index)
        {
            if (IsAnimating)
                actions.Enqueue(new TransitionQueueItem(TransitionQueueItemActionType.Remove, index));
            else
            {
                IsAnimating = true;
                RemoveAndAnimate(index);
            }
        }

        /// <summary>
        /// Inserts the item at the index.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="item">The item</param>
        public void InsertItemOrEnqueue(int index, object item)
        {
            if (IsAnimating)
                actions.Enqueue(new TransitionQueueItem(TransitionQueueItemActionType.Add, index, item));
            else
            {
                IsAnimating = true;
                InsertAndAnimate(index, item);
            }
        }

        private void DequeueActionInternal()
        {
            if (actions.Count > 0)
            {
                var queueItem = actions.Dequeue();
                switch (queueItem.Action)
                {
                    case TransitionQueueItemActionType.Add:
                        this.InsertAndAnimate(queueItem.Index, queueItem.Item);
                        break;
                    case TransitionQueueItemActionType.Remove:
                        this.RemoveAndAnimate(queueItem.Index);
                        break;
                    case TransitionQueueItemActionType.Update:
                        this.SetItem(queueItem.Index, queueItem.Item);
                        break;
                    case TransitionQueueItemActionType.Clear:
                        if (TransitionClearHandling == TransitionClearHandling.Transition)
                            this.ClearItemsOrEnqueue();
                        else
                            this.ClearWithNoAnimation();
                        break;
                }
            }
            else
            {
                IsAnimating = false;
                canAnimate = true;
                OnTransitionCompleted();
            }
        }

        /// <summary>
        /// Cancel animation, clear the queue and go to last action.
        /// </summary>
        public void CancelTransition()
        {
            if (IsAnimating)
            {
                canAnimate = false;
            }
            OnTransitionCancelled();
        }

        /// <summary>
        /// Allows to find the index of the item.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The index or -1</returns>
        public int FindIndex(object item)
        {
            var items = this.innerItemsControl.Items;
            int i = 0;
            foreach (var _item in items)
            {
                if (_item == item)
                    return i;
                i++;
            }
            return -1;
        }

        /// <summary>
        /// Finds the index of the control.
        /// </summary>
        /// <param name="element">The child element</param>
        /// <returns>The index or -1</returns>
        public int FindControlIndex(UIElement element)
        {
            var target = XamlHelper.FindParent<ContentPresenter>(element);
            if (target != null)
                return EnumerableUtils.FindIndex(innerItemsControl.Items, target);

            return -1;
        }

        /// <summary>
        /// Finds the index of the view model.
        /// </summary>
        /// <param name="element">The child element</param>
        /// <returns>The index or -1</returns>
        public int FindContextIndex(UIElement element)
        {
            var target = XamlHelper.FindParent<ContentPresenter>(element);
            if (target != null)
            {
                var context = target.DataContext;
                if (context != null)
                    return EnumerableUtils.FindIndex(innerItemsControl.Items, context);
            }
            return -1;
        }
    }


    internal enum TransitionQueueItemActionType
    {
        Add,
        Remove,
        Update,
        Clear
    }

    internal class TransitionQueueItem
    {
        private TransitionQueueItemActionType action;
        public TransitionQueueItemActionType Action
        {
            get { return action; }
        }

        private int index;
        public int Index
        {
            get { return index; }
        }

        private object item;
        public object Item
        {
            get { return item; }
        }

        public TransitionQueueItem(TransitionQueueItemActionType action, int index, object item)
        {
            this.action = action;
            this.index = index;
            this.item = item;
        }

        public TransitionQueueItem(TransitionQueueItemActionType action, int index)
        {
            this.action = action;
            this.index = index;
        }

        public TransitionQueueItem(TransitionQueueItemActionType action)
        {
            this.action = action;
        }
    }

    /// <summary>
    /// The transition clear handling.
    /// </summary>
    public enum TransitionClearHandling
    {
        /// <summary>
        /// Clear items with transitions.
        /// </summary>
        Transition,
        /// <summary>
        /// Clear items without transitions.
        /// </summary>
        NoTransition,
    }
}
