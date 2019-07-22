using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A collection of <see cref="Behavior"/>.
    /// </summary>
    public sealed class BehaviorCollection : AttachableCollection<Behavior>
    {
        internal BehaviorCollection()
        {
        }

        /// <summary>
        /// Creates the <see cref="Freezable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="BehaviorCollection"/></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new BehaviorCollection();
        }

        internal override void ItemAdded(Behavior navigationBehavior)
        {
            navigationBehavior.Attach(associatedObject);
        }

        internal override void ItemRemoved(Behavior item)
        {
            if (item.AssociatedObject != null)
                item.Detach();
        }

        /// <summary>
        /// Allows to attach <see cref="Behavior"/>.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (Behavior behavior in this)
                behavior.Attach(associatedObject);
        }

        /// <summary>
        /// Allows to detach <see cref="Behavior"/>.
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (Behavior behavior in this)
                behavior.Detach();
        }

    }

}
