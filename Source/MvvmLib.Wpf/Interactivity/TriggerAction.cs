using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// The TriggerAction base class.
    /// </summary>
    public abstract class TriggerAction : Freezable, IAssociatedObject
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
        /// Attaches the trigger action.
        /// </summary>
        /// <param name="associatedObject">The associated object</param>
        public void Attach(DependencyObject associatedObject)
        {
            this.associatedObject = associatedObject;
            OnAttach();
        }

        /// <summary>
        /// Detaches the trigger action.
        /// </summary>
        public void Detach()
        {
            OnDetach();
        }

        /// <summary>
        /// Invoked on attach.
        /// </summary>
        protected virtual void OnAttach()
        {

        }

        /// <summary>
        /// Invoked on detach.
        /// </summary>
        protected virtual void OnDetach()
        {

        }

        internal void InvokeInternal()
        {
            Invoke();
        }

        /// <summary>
        /// Executes the action.
        /// </summary>
        protected abstract void Invoke();
    }

  
}
