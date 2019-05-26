using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation behavior base class.
    /// </summary>
    public abstract class NavigationBehavior : DependencyObject
    {
        private bool isAttached;
        /// <summary>
        /// Checks if attached.
        /// </summary>
        public bool IsAttached
        {
            get { return isAttached; }
        }

        /// <summary>
        /// Attaches the behavior.
        /// </summary>
        public void Attach()
        {
            isAttached = true;
            OnAttach();
        }

        /// <summary>
        /// Detaches th behavior.
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
