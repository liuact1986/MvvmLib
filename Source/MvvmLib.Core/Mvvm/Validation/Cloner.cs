using System;
using System.Collections;
using System.Collections.Generic;

namespace MvvmLib.Mvvm
{

    public class Cloner
    {
        public bool NonPublicConstructors { get; set; }

        public bool NonPublicProperties { get; set; }

        public bool NonPublicFields { get; set; }

        public bool IncludeFields { get; set; }

        private CircularReferenceManager circularReferenceManager;

        public Cloner()
        {
            NonPublicConstructors = true;
            NonPublicProperties = true;
            NonPublicFields = true;
            IncludeFields = false;

            circularReferenceManager = new CircularReferenceManager();
        }

        private object ToDictionary(Type type, IDictionary values)
        {
            var keyType = type.GenericTypeArguments[0];
            var valueType = type.GenericTypeArguments[1];

            var result = ReflectionUtils.CreateDictionary(type, keyType, valueType);

            foreach (DictionaryEntry entry in values)
            {
                var key = entry.Key;
                var value = entry.Value;

                var clonedKey = DoDeepClone(keyType, key);
                var clonedValue = DoDeepClone(valueType, value);

                result.Add(clonedKey, clonedValue);
            }
            return result;
        }

        private object ToListOrCollection(Type type, IList listOrCollection)
        {
            // type => List<IMyItem> ... or List<MyItem>
            var elementType = type.GenericTypeArguments[0];

            var clonedListOrCollection = ReflectionUtils.CreateListOrCollection(type, elementType);
            if (listOrCollection.Count > 0)
            {
                foreach (var value in listOrCollection)
                {
                    var clonedValue = DoDeepClone(elementType, value);
                    clonedListOrCollection.Add(clonedValue);
                }
            }
            return clonedListOrCollection;
        }

        private object ToArray(Type type, Array array)
        {
            var elementType = type.GetElementType();

            var clonedArray = ReflectionUtils.CreateArray(elementType, array.Length);

            for (int i = 0; i < array.Length; i++)
            {
                var value = array.GetValue(i);
                var clonedValue = DoDeepClone(elementType, value);

                clonedArray.SetValue(clonedValue, i);
            }
            return clonedArray;
        }

        private object ToOject(Type type, object instance)
        {
            if (instance == null)
            {
                return null;
            }

            // circular
            var clonedInstanceRegistered = circularReferenceManager.TryGetInstance(instance);
            if (clonedInstanceRegistered != null)
            {
                return clonedInstanceRegistered;
            }

            var clonedInstance = ReflectionUtils.CreateInstance(type, NonPublicConstructors);

            // circular
            circularReferenceManager.AddInstance(instance, clonedInstance);

            // properties
            var properties = ReflectionUtils.GetProperties(type, NonPublicProperties);
            foreach (var property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    var propertyValue = property.GetValue(instance);
                    if (propertyValue != null)
                    {
                        var clonedPropertyValue = DoDeepClone(property.PropertyType, propertyValue);
                        property.SetValue(clonedInstance, clonedPropertyValue);
                    }
                }
            }

            // fields
            if (IncludeFields)
            {
                var fields = ReflectionUtils.GetFields(type, NonPublicFields);
                foreach (var field in fields)
                {
                    var fieldValue = field.GetValue(instance);
                    var clonedFieldValue = DoDeepClone(field.FieldType, fieldValue);

                    field.SetValue(clonedInstance, clonedFieldValue);
                }
            }

            return clonedInstance;
        }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        private bool IsEnumerable(Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        private void ClearCircularReferences()
        {
            circularReferenceManager.Clear();
        }

        private object DoDeepClone(Type type, object value)
        {
            if (value == null) { return null; }

            if (IsValueTypeExtended(type))
            {
                return value;
            }
            else
            {
                // for example: ICollection (property type) => List (concrete type / propertyValue.GetType())
                // for example: IMyItem (property type) => MyItem (concrete type / propertyValue.GetType())
                var concreteType = type.IsInterface ? value.GetType() : type;
                if (IsEnumerable(type))
                {
                    if (type.IsGenericType)
                    {
                        var genericTypeDefinition = type.GetGenericTypeDefinition();
                        if (genericTypeDefinition == typeof(Dictionary<,>)
                            || genericTypeDefinition == typeof(IDictionary<,>))
                        {
                            return ToDictionary(concreteType, (IDictionary)value);
                        }
                        else
                        {
                            return ToListOrCollection(concreteType, (IList)value);
                        }
                    }
                    else if (type.IsArray)
                    {
                        return ToArray(concreteType, (Array)value);
                    }
                }
                else
                {
                    return ToOject(concreteType, value);
                }
            }
            throw new Exception("Cannot clone \"" + type.Name + "\"");
        }

        public object DeepClone(Type type, object value)
        {
            if (value == null) { throw new Exception("Value cannot be null"); }

            ClearCircularReferences();
            return this.DoDeepClone(type, value);
        }

        public T DeepClone<T>(T value)
        {
            return (T)DeepClone(typeof(T), value);
        }
    }

}
