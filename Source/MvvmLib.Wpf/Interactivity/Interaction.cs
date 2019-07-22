using System.Windows;

namespace MvvmLib.Interactivity
{

    /// <summary>
    /// Provides Attached properties for Triggers and Behaviors.
    /// </summary>
    public class Interaction : DependencyObject
    {
        /// <summary>
        /// The behaviors collection.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>An instance of <see cref="BehaviorCollection"/></returns>
        public static BehaviorCollection GetBehaviors(DependencyObject obj)
        {
            var collection = (BehaviorCollection)obj.GetValue(BehaviorsProperty);
            if (collection == null)
            {
                collection = new BehaviorCollection();
                obj.SetValue(BehaviorsProperty, collection);
            }
            return collection;
        }

        /// <summary>
        /// The behaviors collection.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("BehaviorsInternal", typeof(BehaviorCollection), typeof(Interaction), new PropertyMetadata(OnBehaviorsChanged));

        private static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (BehaviorCollection)e.OldValue;
            var newValue = (BehaviorCollection)e.NewValue;
            if (newValue != oldValue)
            {
                if (oldValue != null && oldValue.AssociatedObject != null)
                    oldValue.Detach();

                if (newValue != null && d != null)
                    newValue.Attach(d);
            }
        }

        /// <summary>
        /// The triggers collection.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>An instance of <see cref="TriggerCollection"/></returns>
        public static TriggerCollection GetTriggers(DependencyObject obj)
        {
            var collection = (TriggerCollection)obj.GetValue(TriggersProperty);
            if (collection == null)
            {
                collection = new TriggerCollection();
                obj.SetValue(TriggersProperty, collection);
            }
            return collection;
        }

        /// <summary>
        /// The triggers collection.
        /// </summary>
        public static readonly DependencyProperty TriggersProperty =
            DependencyProperty.RegisterAttached("TriggersInternal", typeof(TriggerCollection), typeof(Interaction), new PropertyMetadata(OnTriggersChanged));

        private static void OnTriggersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (TriggerCollection)e.OldValue;
            var newValue = (TriggerCollection)e.NewValue;
            if (newValue != oldValue)
            {
                if (oldValue != null && oldValue.AssociatedObject != null)
                    oldValue.Detach();

                if (newValue != null && d != null)
                    newValue.Attach(d);
            }
        }
    }
}
