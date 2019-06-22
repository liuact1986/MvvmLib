using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// The navigation behavior base class. Inherits from <see cref="Freezable"/>.
    /// </summary>
    public abstract class NavigationBehavior : Freezable, IAssociatedObject
    {
        /// <summary>
        /// Chekcs if is in design mode.
        /// </summary>
        /// <param name="element">The element</param>
        /// <returns>True if in design mode</returns>
        protected bool IsInDesignMode(DependencyObject element)
        {
            return DesignerProperties.GetIsInDesignMode(element);
        }

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
