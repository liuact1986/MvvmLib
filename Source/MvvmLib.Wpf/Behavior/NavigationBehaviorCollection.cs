using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// The navigation behavior collection.
    /// </summary>
    public class NavigationBehaviorCollection : FreezableCollection<NavigationBehavior>, IEnumerable<KeyValuePair<string, NavigationBehavior>>, IAssociatedObject
    {
        private readonly Dictionary<string, NavigationBehavior> behaviors;

        /// <summary>
        /// Returns the behavior for the key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The behavior</returns>
        public NavigationBehavior this[string key]
        {
            get { return this.behaviors[key]; }
        }

        private DependencyObject associatedObject;
        /// <summary>
        /// The associated object.
        /// </summary>
        public DependencyObject AssociatedObject
        {
            get { return this.associatedObject; }
            set { associatedObject = value; }
        }

        /// <summary>
        /// Creates the navigation behavior collection.
        /// </summary>
        public NavigationBehaviorCollection()
        {
            behaviors = new Dictionary<string, NavigationBehavior>();
        }

        /// <summary>
        /// Adds a behavior to the collection.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="navigationBehavior">The behavior</param>
        public void Add(string key, NavigationBehavior navigationBehavior)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (navigationBehavior == null)
                throw new ArgumentNullException(nameof(navigationBehavior));

            if (this.behaviors.ContainsKey(key))
                throw new ArgumentException($"A behavior with the key \"{key}\" is already registered");

            if (navigationBehavior is IAssociatedObject)
                ((IAssociatedObject)navigationBehavior).AssociatedObject = associatedObject;

            this.behaviors.Add(key, navigationBehavior);

            navigationBehavior.Attach();
        }

        /// <summary>
        /// Checks if a behavior is registered with the key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns></returns>
        public bool ContainsKey(string key)
        {
            return this.behaviors.ContainsKey(key);
        }

        /// <summary>
        /// Attach the behavior with the dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                    throw new InvalidOperationException();

                this.associatedObject = dependencyObject;
            }
        }

        /// <summary>
        /// Detach the behavior.
        /// </summary>
        public void Detach()
        {
            this.associatedObject = null;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        public new IEnumerator<KeyValuePair<string, NavigationBehavior>> GetEnumerator()
        {
            return this.behaviors.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.behaviors.GetEnumerator();
        }
    }
}
