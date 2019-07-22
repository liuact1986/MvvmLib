using MvvmLib.Utils;
using System.Windows;
using System.Windows.Markup;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// The trigger base class.
    /// </summary>
    [ContentProperty("Actions")]
    public abstract class TriggerBase : Freezable, IAssociatedObject
    {
        /// <summary>
        /// The associated dependency object.
        /// </summary>
        protected DependencyObject associatedObject;

        /// <summary>
        /// The associated dependency object.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get { return associatedObject; }
        }

        /// <summary>
        /// Gets the trigger actions.
        /// </summary>
        /// <param name="obj">The object</param>
        /// <returns>The trigger actions collection</returns>
        public static TriggerActionCollection GetTriggerActions(DependencyObject obj)
        {
            var collection = (TriggerActionCollection)obj.GetValue(TriggerActionsProperty);
            if (collection == null)
            {
                collection = new TriggerActionCollection();
                obj.SetValue(TriggerActionsProperty, collection);
            }
            return collection;
        }

        /// <summary>
        /// Gets the trigger actions.
        /// </summary>
        public static readonly DependencyProperty TriggerActionsProperty =
            DependencyProperty.RegisterAttached("TriggerActionsInternal", typeof(TriggerActionCollection), typeof(TriggerBase), new PropertyMetadata());

        /// <summary>
        /// Gets the trigger actions.
        /// </summary>
        public TriggerActionCollection Actions
        {
            get { return GetTriggerActions(this); }
        }

        /// <summary>
        /// Attaches the trigger.
        /// </summary>
        /// <param name="associatedObject">The associated object</param>
        public void Attach(DependencyObject associatedObject)
        {
            this.associatedObject = associatedObject;
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                this.Actions.Attach(associatedObject);
                OnAttach();
            }
        }

        /// <summary>
        /// Detaches the trigger.
        /// </summary>
        public void Detach()
        {
            OnDetach();
        }

        /// <summary>
        /// Invokes the actions.
        /// </summary>
        protected void InvokeActions()
        {
            foreach (var action in Actions)
            {
                action.InvokeInternal();
            }
        }

        /// <summary>
        /// Invoked on attach.
        /// </summary>
        protected abstract void OnAttach();

        /// <summary>
        /// Invoked on detach.
        /// </summary>
        protected abstract void OnDetach();
    }
}
