using MvvmLib.Utils;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// The behavior base class. Inherits from <see cref="Freezable"/>.
    /// </summary>
    public abstract class Behavior : Freezable, IAssociatedObject
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
        /// Attaches the behavior.
        /// </summary>
        public void Attach(DependencyObject associatedObject)
        {
            this.associatedObject = associatedObject;
            if (!DesignModeHelper.IsInDesignMode(this))
            {
                OnAttach();
            }
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
