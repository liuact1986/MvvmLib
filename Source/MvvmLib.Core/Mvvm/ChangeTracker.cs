using MvvmLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Allows to track object changes.
    /// </summary>
    public class ChangeTracker : INotifyPropertyChanged
    {
        private object originalSource;
        private Dictionary<string, ChangeTrackedProperty> trackedProperties;
        private readonly Cloner cloner;
        private readonly List<string> propertiesToIgnore;
        private bool isEnumerableSource;
        private TrackedSource trackedSource;

        private bool hasChanges;
        /// <summary>
        /// Checks if the source provived has changes.
        /// </summary>
        public bool HasChanges
        {
            get { return hasChanges; }
            private set
            {
                if (hasChanges != value)
                {
                    hasChanges = value;
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        /// <summary>
        /// Checks if a source is provided.
        /// </summary>
        public bool CanCheckChanges
        {
            get { return this.originalSource != null; }
        }

        /// <summary>
        /// Invoked on property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates the <see cref="ChangeTracker"/>.
        /// </summary>
        /// <param name="propertiesToIgnore">The properties to ignore</param>
        public ChangeTracker(List<string> propertiesToIgnore)
        {
            this.cloner = new Cloner();
            this.propertiesToIgnore = propertiesToIgnore;
        }

        /// <summary>
        /// Creates the <see cref="ChangeTracker"/>.
        /// </summary>
        public ChangeTracker()
            : this(null)
        { }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        /// <summary>
        /// Starts tracking changes.
        /// </summary>
        /// <param name="originalSource">The original source</param>
        public void TrackChanges(object originalSource)
        {
            // clone and track properties
            if (originalSource == null)
                throw new ArgumentNullException(nameof(originalSource));

            if (this.originalSource != null)
                UnhandlePropertyChange();

            var type = originalSource.GetType();
            // value ?
            if (IsValueTypeExtended(type))
                throw new NotSupportedException("Only enumerables and objects are supported for original source");

            this.isEnumerableSource = false;
            this.trackedSource = null;
            if (this.trackedProperties != null)
                this.trackedProperties.Clear();

            this.originalSource = originalSource;

            // enumerable
            if (ReflectionUtils.IsEnumerableType(type))
            {
                var clonedValue = this.cloner.DeepClone(originalSource);
                this.isEnumerableSource = true;
                this.trackedSource = new TrackedSource(originalSource, clonedValue);
            }
            else
            {
                this.trackedProperties = new Dictionary<string, ChangeTrackedProperty>();
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite && (propertiesToIgnore == null || !propertiesToIgnore.Contains(property.Name)))
                    {
                        var propertyType = property.PropertyType;
                        if (IsValueTypeExtended(propertyType))
                        {
                            var value = property.GetValue(originalSource);
                            this.trackedProperties[property.Name] = new ChangeTrackedProperty(property, value, ChangeTrackedPropertyType.Value);
                        }
                        else if (ReflectionUtils.IsEnumerableType(propertyType))
                        {
                            var value = property.GetValue(originalSource);
                            var clonedValue = value == null ? null : this.cloner.DeepClone(value);
                            this.trackedProperties[property.Name] = new ChangeTrackedProperty(property, clonedValue, ChangeTrackedPropertyType.Enumerable);
                        }
                        else
                        {
                            var value = property.GetValue(originalSource);
                            if (!(ReflectionUtils.IsCommandType(propertyType)))
                            {
                                var clonedValue = value == null ? null : this.cloner.DeepClone(value);
                                this.trackedProperties[property.Name] = new ChangeTrackedProperty(property, clonedValue, ChangeTrackedPropertyType.Object);
                            }
                        }
                    }
                }
                HandlePropertyChange();
            }
        }

        /// <summary>
        /// Handles <see cref="INotifyPropertyChanged"/> for the original source provided.
        /// </summary>
        public void HandlePropertyChange()
        {
            if (originalSource is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)originalSource).PropertyChanged += OnPropertyChanged;
            }
        }

        /// <summary>
        /// Unhandles <see cref="INotifyPropertyChanged"/> for the original source provided.
        /// </summary>
        public void UnhandlePropertyChange()
        {
            if (originalSource is INotifyPropertyChanged)
            {
                ((INotifyPropertyChanged)originalSource).PropertyChanged -= OnPropertyChanged;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckChanges(e.PropertyName);
        }

        private void CheckObjectHasChanged(ChangeTrackedProperty trackedProperty)
        {
            bool hasChanged = ObjectHasChanged(trackedProperty);
            TryUpdateHasChanges(hasChanged);
        }

        private void CheckEnumerableHasChanged(ChangeTrackedProperty trackedProperty)
        {
            bool hasChanged = EnumerableHasChanged(trackedProperty);
            TryUpdateHasChanges(hasChanged);
        }

        private void CheckValueHasChanged(ChangeTrackedProperty trackedProperty)
        {
            bool hasChanged = ValueHasChanged(trackedProperty);
            TryUpdateHasChanges(hasChanged);
        }

        private bool ObjectHasChanged(ChangeTrackedProperty trackedProperty)
        {
            var newValue = trackedProperty.Property.GetValue(this.originalSource);
            var oldValue = trackedProperty.ClonedValue;
            var hasChanged = ObjectHasChanged(oldValue, newValue);
            trackedProperty.HasChanges = hasChanged;
            return hasChanged;
        }

        private bool EnumerableHasChanged(ChangeTrackedProperty trackedProperty)
        {
            var newValue = trackedProperty.Property.GetValue(this.originalSource);
            var oldValue = trackedProperty.ClonedValue;
            var hasChanged = EnumerableHasChanged(oldValue, newValue);
            trackedProperty.HasChanges = hasChanged;
            return hasChanged;
        }

        private bool ValueHasChanged(ChangeTrackedProperty trackedProperty)
        {
            var newValue = trackedProperty.Property.GetValue(this.originalSource);
            var oldValue = trackedProperty.ClonedValue;
            var hasChanged = ValueHasChanged(oldValue, newValue);
            trackedProperty.HasChanges = hasChanged;
            return hasChanged;
        }

        private void TryUpdateHasChanges(bool hasChanged)
        {
            if (hasChanged)
            {
                if (!HasChanges)
                    HasChanges = true;
            }
            else
            {
                if (HasChanges)
                {
                    foreach (var trackedProperty in trackedProperties)
                    {
                        if (trackedProperty.Value.HasChanges)
                            return;
                    }
                    HasChanges = false;
                }
            }
        }

        private bool ValueHasChanged(object oldValue, object newValue)
        {
            if (newValue == null)
                return !(oldValue == null);

            var equals = newValue.Equals(oldValue);
            return !equals;
        }

        private bool EnumerableHasChanged(object oldValue, object newValue)
        {
            // 1. null ?
            if (oldValue == null)
                return !(newValue == null);
            else if (newValue == null)
                return !(oldValue == null);

            // 2. Count 
            var type = oldValue.GetType();
            var isArray = type.IsArray;
            if (!isArray)
            {
                var sizePropertyName = "Count";
                var sizeProperty = type.GetProperty(sizePropertyName);
                if (sizeProperty != null)
                {
                    var oldSize = sizeProperty.GetValue(oldValue);
                    var newSize = sizeProperty.GetValue(newValue);
                    if (!oldSize.Equals(newSize))
                        return true;
                }
            }

            // 3. value changed

            var oldEnumerator = ((IEnumerable)oldValue).GetEnumerator();
            var newEnumerator = ((IEnumerable)newValue).GetEnumerator();

            while (true)
            {
                bool oldMoveNext = oldEnumerator.MoveNext();
                newEnumerator.MoveNext();

                if (!oldMoveNext)
                    break;

                var nextOldValue = oldEnumerator.Current;
                var nextNewValue = newEnumerator.Current;

                // assume that we reach the "end" of array
                if (isArray && nextOldValue == null && nextNewValue == null)
                    break;

                if (AnyHasChanged(nextOldValue, nextNewValue))
                    return true;
            }

            return false;
        }

        private bool ObjectHasChanged(object oldValue, object newValue)
        {
            if (oldValue == null)
                return !(newValue == null);
            else if (newValue == null)
                return !(oldValue == null);

            var properties = oldValue.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyType = property.PropertyType;
                var nextOldValue = property.GetValue(oldValue);
                var nextNewValue = property.GetValue(newValue);

                if (IsValueTypeExtended(propertyType))
                {
                    // value comparison
                    if (ValueHasChanged(nextOldValue, nextNewValue))
                        return true;
                }
                else if (ReflectionUtils.IsEnumerableType(propertyType))
                {

                    if (EnumerableHasChanged(nextOldValue, nextNewValue))
                        return true;
                }
                else
                {
                    if (!(ReflectionUtils.IsCommandType(propertyType)))
                    {
                        // can be null
                        if (ObjectHasChanged(nextOldValue, nextNewValue))
                            return true;
                    }
                }
            }
            return false;
        }

        private bool AnyHasChanged(object oldValue, object newValue)
        {
            if (oldValue == null)
            {
                return !(newValue == null);
            }
            else if (newValue == null)
            {
                return !(oldValue == null);
            }

            var type = oldValue.GetType();
            if (IsValueTypeExtended(type))
            {
                // used for Dictionary KeyValuePair
                if (ValueHasChanged(oldValue, newValue))
                    return true;
            }
            else if (ReflectionUtils.IsEnumerableType(type))
            {
                if (EnumerableHasChanged(oldValue, newValue))
                    return true;
            }
            else
            {
                if (ObjectHasChanged(oldValue, newValue))
                    return true;
            }

            return false;
        }

        private bool CheckChangesWithReflection()
        {
            if (!this.CanCheckChanges)
                throw new InvalidOperationException("No source provided. Cannot check changes. Call 'TrackChanges' to provide an original source");

            if (this.isEnumerableSource)
            {
                var oldValue = this.trackedSource.ClonedValue;
                var newValue = this.trackedSource.Value;

                var hasChanged = EnumerableHasChanged(oldValue, newValue);
                this.HasChanges = hasChanged;
                return hasChanged;
            }
            else
            {
                foreach (var trackedProperty in trackedProperties.Values)
                {
                    switch (trackedProperty.PropertyType)
                    {
                        case ChangeTrackedPropertyType.Value:
                            if (ValueHasChanged(trackedProperty))
                                return true;
                            break;
                        case ChangeTrackedPropertyType.Enumerable:
                            if (EnumerableHasChanged(trackedProperty))
                                return true;
                            break;
                        case ChangeTrackedPropertyType.Object:
                            if (ObjectHasChanged(trackedProperty))
                                return true;
                            break;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks changes for the property.
        /// </summary>
        /// <returns>True if has changes</returns>
        public bool CheckChanges(string propertyName)
        {
            if (!this.CanCheckChanges)
                throw new InvalidOperationException("No source provided. Cannot check changes. Call 'TrackChanges' to provide an original source");
            if (this.isEnumerableSource)
                throw new InvalidOperationException("The source is enumerable");

            if (this.trackedProperties.TryGetValue(propertyName, out ChangeTrackedProperty trackedProperty))
            {
                switch (trackedProperty.PropertyType)
                {
                    case ChangeTrackedPropertyType.Value:
                        CheckValueHasChanged(trackedProperty);
                        break;
                    case ChangeTrackedPropertyType.Enumerable:
                        CheckEnumerableHasChanged(trackedProperty);
                        break;
                    case ChangeTrackedPropertyType.Object:
                        CheckObjectHasChanged(trackedProperty);
                        break;
                }
            }

            return hasChanges;
        }

        /// <summary>
        /// Checks changes.
        /// </summary>
        /// <returns>True if has changes</returns>
        public bool CheckChanges()
        {
            bool hasChanged = CheckChangesWithReflection();
            TryUpdateHasChanges(hasChanged);
            return hasChanged;
        }

        /// <summary>
        /// Accept changes and set has changes to false.
        /// </summary>
        public void AcceptChanges()
        {
            TrackChanges(this.originalSource);
            this.HasChanges = false;
        }
    }

    /// <summary>
    /// The tracked property type.
    /// </summary>
    public enum ChangeTrackedPropertyType
    {
        /// <summary>
        /// Value
        /// </summary>
        Value,
        /// <summary>
        /// Enumerable
        /// </summary>
        Enumerable,
        /// <summary>
        /// Object
        /// </summary>
        Object
    }

    /// <summary>
    /// A class with property and original value.
    /// </summary>
    public class ChangeTrackedProperty
    {
        private PropertyInfo property;
        /// <summary>
        /// The property.
        /// </summary>
        public PropertyInfo Property
        {
            get { return property; }
        }

        private object clonedValue;
        /// <summary>
        /// The cloned value.
        /// </summary>
        public object ClonedValue
        {
            get { return clonedValue; }
        }

        private bool hasChanges;
        /// <summary>
        /// Chekcs if the current property has changes.
        /// </summary>
        public bool HasChanges
        {
            get { return hasChanges; }
            set { hasChanges = value; }
        }

        private ChangeTrackedPropertyType propertyType;
        /// <summary>
        /// The <see cref="ChangeTrackedPropertyType"/>.
        /// </summary>
        public ChangeTrackedPropertyType PropertyType
        {
            get { return propertyType; }
        }

        /// <summary>
        /// Creates the <see cref="ChangeTrackedProperty"/>.
        /// </summary>
        /// <param name="property">The property</param>
        /// <param name="clonedValue">The cloned value</param>
        /// <param name="propertyType">The property type</param>
        public ChangeTrackedProperty(PropertyInfo property, object clonedValue, ChangeTrackedPropertyType propertyType)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            this.property = property;
            this.clonedValue = clonedValue;
            this.propertyType = propertyType;
        }
    }


    /// <summary>
    /// A tracked source.
    /// </summary>
    public class TrackedSource
    {
        private object value;
        /// <summary>
        /// The value.
        /// </summary>
        public object Value
        {
            get { return value; }
        }

        private object clonedValue;
        /// <summary>
        /// The cloned value.
        /// </summary>
        public object ClonedValue
        {
            get { return clonedValue; }
        }

        /// <summary>
        /// Creates the <see cref="TrackedSource"/>.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="clonedValue">The cloned value</param>
        public TrackedSource(object value, object clonedValue)
        {
            this.value = value;
            this.clonedValue = clonedValue;
        }
    }
}
