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
        private readonly Cloner cloner;
        private readonly Dictionary<Type, PropertyInfo[]> propertyCache;
        private readonly object originalValue;
        private readonly bool keepAlive;
        private object trackedValue;
        private WeakReference trackedValueRef;

        private bool nonPublicProperties;
        /// <summary>
        /// Allows to include non public properties.
        /// </summary>
        public bool NonPublicProperties
        {
            get { return nonPublicProperties; }
            set { nonPublicProperties = value; }
        }

        private bool hasChanges;
        /// <summary>
        /// Checks if the value has changed.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return hasChanges;
            }
            private set
            {
                if (value != hasChanges)
                {
                    hasChanges = value;
                    OnPropertyChanged(nameof(HasChanges));
                }
            }
        }

        private ChangeTrackerMode trackerMode;
        /// <summary>
        /// The <see cref="ChangeTrackerMode"/> (<see cref="ChangeTrackerMode.UseReflection"/> by default).
        /// </summary>
        public ChangeTrackerMode TrackerMode
        {
            get { return trackerMode; }
            set { trackerMode = value; }
        }

        /// <summary>
        /// Used for <see cref="HasChanges"/>.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates the <see cref="ChangeTracker"/>.
        /// </summary>
        /// <param name="originalValue">The original value</param>
        /// <param name="keepAlive">Use a <see cref="WeakReference"/> if False</param>
        public ChangeTracker(object originalValue, bool keepAlive = true)
        {
            if (originalValue == null)
                throw new ArgumentNullException(nameof(originalValue));

            this.cloner = new Cloner();
            this.propertyCache = new Dictionary<Type, PropertyInfo[]>();
            this.nonPublicProperties = true;
            this.trackerMode = ChangeTrackerMode.UseReflection;
            this.keepAlive = keepAlive;

            this.originalValue = cloner.DeepClone(originalValue);
            if (keepAlive)
                this.trackedValue = originalValue;
            else
                this.trackedValueRef = new WeakReference(originalValue);
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        private PropertyInfo[] GetProperties(Type type)
        {
            if (propertyCache.TryGetValue(type, out PropertyInfo[] properties))
            {
                return properties;
            }
            else
            {
                properties = ReflectionUtils.GetProperties(type, nonPublicProperties);
                propertyCache[type] = properties;
                return properties;
            }
        }

        private bool ValueHasChanged(object oldValue, object newValue)
        {
            var equals = newValue.Equals(oldValue);
            return !equals;
        }

        private bool EnumerableHasChanged(object oldValue, object newValue)
        {
            // 1. null ?
            if (oldValue == null)
            {
                return !(newValue == null);
            }
            else if (newValue == null)
            {
                return !(oldValue == null);
            }

            // 2. Count

            var type = oldValue.GetType();
            var isArray = type.IsArray;
            if (!isArray)
            {
                var countProperty = type.GetProperty("Count");
                if (countProperty != null)
                {
                    var oldCount = countProperty.GetValue(oldValue);
                    var newCount = countProperty.GetValue(newValue);
                    if (!oldCount.Equals(newCount))
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
            {
                return !(newValue == null);
            }
            else if (newValue == null)
            {
                return !(oldValue == null);
            }

            var properties = GetProperties(oldValue.GetType());
            foreach (var property in properties)
            {
                var type = property.PropertyType;
                var nextOldValue = property.GetValue(oldValue);
                var nextNewValue = property.GetValue(newValue);

                if (IsValueTypeExtended(type))
                {
                    // value comparison
                    if (ValueHasChanged(nextOldValue, nextNewValue))
                        return true;
                }
                else if (ReflectionUtils.IsEnumerableType(type))
                {

                    if (EnumerableHasChanged(nextOldValue, nextNewValue))
                        return true;
                }
                else
                {
                    // can be null
                    if (ObjectHasChanged(nextOldValue, nextNewValue))
                        return true;
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
            bool changed = false;
            var type = this.originalValue.GetType();
            var trackedValue = keepAlive ? this.trackedValue : this.trackedValueRef.Target;
            if (IsValueTypeExtended(type))
                throw new NotSupportedException($"Type \"{type.Name}\" not supported");
            else if (ReflectionUtils.IsEnumerableType(type))
                changed = EnumerableHasChanged(this.originalValue, trackedValue);
            else
                changed = ObjectHasChanged(this.originalValue, trackedValue);

            return changed;
        }

        private bool CheckChangesWithEquals()
        {
            var trackedValue = keepAlive ? this.trackedValue : this.trackedValueRef.Target;
            var equals = this.originalValue.Equals(trackedValue);
            return !equals;
        }

        /// <summary>
        /// Checks changes.
        /// </summary>
        /// <returns>True if has changes</returns>
        public bool CheckChanges()
        {
            bool changed = false;
            switch (trackerMode)
            {
                case ChangeTrackerMode.UseEquals:
                    changed = CheckChangesWithEquals();
                    break;
                case ChangeTrackerMode.UseReflection:
                    changed = CheckChangesWithReflection();
                    break;
                default:
                    throw new NotSupportedException("Unexpected TrackerMode");
            }

            this.HasChanges = changed;

            return changed;
        }
    }

    /// <summary>
    /// The tracker Mode
    /// </summary>
    public enum ChangeTrackerMode
    {
        /// <summary>
        /// Use Equals.
        /// </summary>
        UseEquals,
        /// <summary>
        /// Use Reflection.
        /// </summary>
        UseReflection
    }
}
