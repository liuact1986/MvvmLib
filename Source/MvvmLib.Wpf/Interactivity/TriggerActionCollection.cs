using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A collection of <see cref="TriggerAction"/>.
    /// </summary>
    public class TriggerActionCollection : AttachableCollection<TriggerAction>
    {
        internal TriggerActionCollection()
        {

        }

        /// <summary>
        /// Creates the <see cref="TriggerActionCollection"/>.
        /// </summary>
        /// <returns>The freezable</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new TriggerActionCollection();
        }

        /// <summary>
        /// Attaches the trigger actions.
        /// </summary>
        protected override void OnAttached()
        {
            foreach (TriggerAction triggerAction in this)
            {
                triggerAction.Attach(base.AssociatedObject);
            }
        }

        /// <summary>
        /// Detaches the trigger actions.
        /// </summary>
        protected override void OnDetaching()
        {
            foreach (TriggerAction triggerAction in this)
            {
                triggerAction.Detach();
            }
        }

        internal override void ItemAdded(TriggerAction triggerAction)
        {
            if (base.AssociatedObject != null)
            {
                triggerAction.Attach(base.AssociatedObject);
            }
        }

        internal override void ItemRemoved(TriggerAction triggerAction)
        {
            if(triggerAction.AssociatedObject != null)
            {
                triggerAction.Detach();
            }
        }
    }
}
