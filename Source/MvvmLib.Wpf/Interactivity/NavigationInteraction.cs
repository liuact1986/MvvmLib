using System.Windows;

namespace MvvmLib.Interactivity
{

    /// <summary>
    /// Provides Attached properties for <see cref="NavigationBehavior"/>.
    /// </summary>
    public class NavigationInteraction : DependencyObject
    {
        /// <summary>
        /// The behaviors collection.
        /// </summary>
        /// <param name="obj">The dependency object</param>
        /// <returns>An instance of <see cref="NavigationBehaviorCollection"/></returns>
        public static NavigationBehaviorCollection GetBehaviors(DependencyObject obj)
        {
            var collection = (NavigationBehaviorCollection)obj.GetValue(BehaviorsProperty);
            if (collection == null)
            {
                collection = new NavigationBehaviorCollection();
                obj.SetValue(BehaviorsProperty, collection);
            }
            return collection;
        }

        /// <summary>
        /// The behaviors collection.
        /// </summary>
        public static readonly DependencyProperty BehaviorsProperty =
            DependencyProperty.RegisterAttached("BehaviorsInternal", typeof(NavigationBehaviorCollection), typeof(NavigationInteraction), new PropertyMetadata(OnBehaviorsChanged));

        private static void OnBehaviorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldValue = (NavigationBehaviorCollection)e.OldValue;
            var newValue = (NavigationBehaviorCollection)e.NewValue;
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
