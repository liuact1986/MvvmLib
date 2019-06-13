using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;

namespace MvvmLib.Interactivity
{
    /// <summary>
    /// A collection that inherits from <see cref="FreezableCollection{T}"/>. The base class for <see cref="NavigationBehaviorCollection"/>.
    /// </summary>
    /// <typeparam name="T">The parameter type</typeparam>
    public abstract class AttachableCollection<T> : FreezableCollection<T>, IAssociatedObject where T : DependencyObject
    {
        private Collection<T> snapshot;

        /// <summary>
        /// Gets the associated object.
        /// </summary>
        protected DependencyObject associatedObject;
        /// <summary>
        /// Gets the associated object.
        /// </summary>
        /// <value>The associated object.</value>
        public DependencyObject AssociatedObject
        {
            get { return this.associatedObject; }
            set { associatedObject = value; }
        }

        internal AttachableCollection()
        {
            this.snapshot = new Collection<T>();
            ((INotifyCollectionChanged)this).CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// Attaches to the object.
        /// </summary>
        /// <param name="dependencyObject">The object to attach to</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                throw new ArgumentNullException(nameof(dependencyObject));

            if (dependencyObject != this.AssociatedObject)
            {
                if (this.AssociatedObject != null)
                    throw new InvalidOperationException();

                if (!(bool)base.GetValue(DesignerProperties.IsInDesignModeProperty))
                {
                    base.WritePreamble();
                    this.associatedObject = dependencyObject;
                    base.WritePostscript();
                }
                this.OnAttached();
            }
        }

        /// <summary>
        /// Detaches the instance from the associated object.
        /// </summary>
        public void Detach()
        {
            this.OnDetaching();
            base.WritePreamble();
            this.associatedObject = null;
            base.WritePostscript();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (T item in e.NewItems)
                    {
                        this.ItemAdded(item);
                        this.snapshot.Insert(base.IndexOf(item), item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (T item in e.OldItems)
                    {
                        this.ItemRemoved(item);
                        this.snapshot.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (T oldItem in e.OldItems)
                    {
                        this.ItemRemoved(oldItem);
                        this.snapshot.Remove(oldItem);
                    }
                    foreach (T item in e.NewItems)
                    {
                        this.ItemAdded(item);
                        this.snapshot.Insert(base.IndexOf(item), item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    return;
                case NotifyCollectionChangedAction.Reset:
                    foreach (T item in this.snapshot)
                    {
                        this.ItemRemoved(item);
                    }

                    this.snapshot = new Collection<T>();
                    break;
                default:
                    return;
            }
        }


        internal abstract void ItemAdded(T item);
        internal abstract void ItemRemoved(T item);
        /// <summary>
        /// Invoked on attach.
        /// </summary>
        protected abstract void OnAttached();
        /// <summary>
        /// Invoked on detach.
        /// </summary>
        protected abstract void OnDetaching();
    }

}
