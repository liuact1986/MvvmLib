using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A collection of <see cref="TriggerBase"/>.
    /// </summary>
    public sealed class TriggerCollection : AttachableCollection<TriggerBase>
    {
        internal TriggerCollection()
        {
        }

        /// <summary>
        /// Creates the <see cref="TriggerCollection"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new TriggerCollection();
        }

        internal override void ItemAdded(TriggerBase item)
        {
            item.Attach(associatedObject);
        }

        internal override void ItemRemoved(TriggerBase item)
        {
            if (item.AssociatedObject != null)
                item.Detach();
        }

        /// <summary>
        /// Attaches the triggers.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (TriggerBase item in this)
                item.Attach(associatedObject);
        }

        /// <summary>
        /// Detaches the triggers.
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (TriggerBase item in this)
                item.Detach();
        }

    }

   
}
