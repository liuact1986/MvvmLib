using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// A collection of <see cref="NavigationBehavior"/>.
    /// </summary>
    public sealed class NavigationBehaviorCollection : AttachableCollection<NavigationBehavior>
    {
        internal NavigationBehaviorCollection()
        {
        }

        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="NavigationBehaviorCollection"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new NavigationBehaviorCollection();
        }

        internal override void ItemAdded(NavigationBehavior navigationBehavior)
        {
            navigationBehavior.Attach(associatedObject);
        }

        internal override void ItemRemoved(NavigationBehavior item)
        {
            if (item.AssociatedObject != null)
                item.Detach();
        }

        /// <summary>
        /// Allows to attach <see cref="NavigationBehavior"/>.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (NavigationBehavior behavior in this)
                behavior.Attach(associatedObject);
        }

        /// <summary>
        /// Allows to detach <see cref="NavigationBehavior"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (NavigationBehavior behavior in this)
                behavior.Detach();
        }

    }

}
