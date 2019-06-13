using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// The navigation behavior base class. Inherits from <see cref="Freezable"/>.
    /// </summary>
    public abstract class NavigationBehavior : Freezable, IAssociatedObject
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
            set { associatedObject = value; }
        }

        /// <summary>
        /// Attaches the behavior.
        /// </summary>
        public void Attach(DependencyObject associatedObject)
        {
            this.associatedObject = associatedObject;
            OnAttach();
        }

        /// <summary>
        /// Detaches the behavior.
        /// </summary>
        public void Detach()
        {
            OnDetach();
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
