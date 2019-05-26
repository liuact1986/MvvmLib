using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// Provides Attached properties for navigation behaviors.
    /// </summary>
    public class NavigationInteraction : DependencyObject
    {

        public static bool GetSelectionChangedBehavior(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectionChangedBehaviorProperty);
        }

        public static void SetSelectionChangedBehavior(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectionChangedBehaviorProperty, value);
        }

        public static readonly DependencyProperty SelectionChangedBehaviorProperty =
            DependencyProperty.RegisterAttached("SelectionChangedBehavior", typeof(bool), typeof(NavigationInteraction), new PropertyMetadata(false, OnSelectionChangedBehaviorChanged));

        private static void OnSelectionChangedBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Selector))
                throw new InvalidOperationException($"Expected type Selector (ListBox, TabControl, etc.). Current Type \"{d.GetType()}\"");

            var behavior = new SelectionChangedBehavior { AssociatedObject = d };
            behavior.Attach();
        }

        public static NavigationBehaviorCollection GetBehaviors(DependencyObject obj)
        {
            return (NavigationBehaviorCollection)obj.GetValue(BehaviorsProperty);
        }

        public static void SetBehaviors(DependencyObject obj, NavigationBehaviorCollection value)
        {
            obj.SetValue(BehaviorsProperty, value);
        }

        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("Behaviors", typeof(NavigationBehaviorCollection), typeof(NavigationInteraction), new PropertyMetadata(new NavigationBehaviorCollection(), OnBehaviorsChanged));

        private static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (NavigationBehaviorCollection)e.OldValue;
            var newValue = (NavigationBehaviorCollection)e.NewValue;
            if (oldValue != newValue)
            {
                if (oldValue != null && ((IAssociatedObject)oldValue).AssociatedObject != null)
                    oldValue.Detach();

                if (newValue != null && d != null)
                {
                    if (((IAssociatedObject)newValue).AssociatedObject != null)
                        throw new InvalidOperationException();

                    newValue.Attach(d);
                }
            }
        }
    }
}
