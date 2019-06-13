using MvvmLib.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MvvmLib.Mvvm
{

    /// <summary>
    /// Allows to clone values or objects.
    /// </summary>
    public class Cloner
    {
        private readonly CircularReferenceManager circularReferenceManager;
        private readonly DefaultDelegateFactory delegateFactory;
        private readonly MethodInfo cloneGenericDictionaryMethod;
        private readonly MethodInfo cloneGenericListOrCollectionMethod;

        private List<string> blackList;
        /// <summary>
        /// Allows to ignore field or property by name.
        /// </summary>
        public List<string> BlackList
        {
            get { return blackList; }
        }

        private bool nonPublicConstructors;
        /// <summary>
        /// Allows to include non public constructors (true by default).
        /// </summary>
        public bool NonPublicConstructors
        {
            get { return nonPublicConstructors; }
            set { nonPublicConstructors = value; }
        }

        private bool nonPublicProperties;
        /// <summary>
        /// Allows to include non public properties (true by default).
        /// </summary>
        public bool NonPublicProperties
        {
            get { return nonPublicProperties; }
            set { nonPublicProperties = value; }
        }

        private bool includeFields;
        /// <summary>
        /// Allows to include fields (false by default).
        /// </summary>
        public bool IncludeFields
        {
            get { return includeFields; }
            set { includeFields = value; }
        }

        private bool includeDelegates;
        /// <summary>
        /// Allows to clone <see cref="Delegate"/> (false by default). 
        /// </summary>
        public bool IncludeDelegates
        {
            get { return includeDelegates; }
            set { includeDelegates = value; }
        }

        private ClonerErrorHandling errorHandling;
        /// <summary>
        /// Error handling (<see cref="ClonerErrorHandling.UseOriginalValue"/> by default).
        /// </summary>
        public ClonerErrorHandling ErrorHandling
        {
            get { return errorHandling; }
            set { errorHandling = value; }
        }

        /// <summary>
        /// Creates the cloner.
        /// </summary>
        public Cloner()
        {
            this.nonPublicConstructors = true;
            this.nonPublicProperties = true;
            this.includeFields = false;
            this.errorHandling = ClonerErrorHandling.UseOriginalValue;
            this.includeDelegates = false;
            this.blackList = new List<string>();
            this.circularReferenceManager = new CircularReferenceManager();
            this.delegateFactory = new DefaultDelegateFactory();

            this.cloneGenericDictionaryMethod = typeof(Cloner).GetMethod("CloneGenericDictionary", BindingFlags.Instance | BindingFlags.NonPublic);
            this.cloneGenericListOrCollectionMethod = typeof(Cloner).GetMethod("CloneGenericListOrCollection", BindingFlags.Instance | BindingFlags.NonPublic);

        }

        private Delegate CloneDelegate(Delegate @delegate)
        {
            var method = @delegate.Method;
            var target = @delegate.Target;
            var clone = Delegate.CreateDelegate(@delegate.GetType(), target, method);
            return clone;
        }

        private object CreateInstance(Type type)
        {
            try
            {
                var flags = ReflectionUtils.GetFlags(nonPublicConstructors);
                var emptyConstructor = type.GetConstructor(flags, null, Type.EmptyTypes, null);
                if (emptyConstructor != null)
                {
                    var instance = emptyConstructor.Invoke(null);
                    return instance;
                }
                else
                {
                    var constructor = type.GetConstructors(flags).FirstOrDefault();
                    if (constructor != null)
                    {
                        var parameters = constructor.GetParameters();
                        var fakeParameters = new object[parameters.Length];
                        int index = 0;
                        foreach (ParameterInfo parameter in parameters)
                        {
                            var parameterType = parameter.ParameterType;
                            if (parameterType.IsValueType)
                            {
                                fakeParameters[index] = Activator.CreateInstance(parameterType);
                            }
                            else if (parameterType == typeof(string))
                            {
                                fakeParameters[index] = ""; // avoid ArgumentNullException
                            }
                            else
                            {
                                if (ReflectionUtils.IsDelegateType(parameterType))
                                {
                                    if (!includeDelegates)
                                    {
                                        return null;
                                    }
                                    else
                                    {
                                        if (ReflectionUtils.IsActionType(parameterType))
                                        {
                                            if (parameterType.IsGenericType)
                                            {
                                                var types = parameterType.GetGenericArguments();
                                                if (types.Length > 8)
                                                    throw new NotSupportedException($"Actions with less than 9 args supported '{parameter.Name}'");

                                                var methodName = $"GetAction{types.Length}";

                                                var method = typeof(DefaultDelegateFactory).GetMethod(methodName);
                                                var genericMethod = method.MakeGenericMethod(types);
                                                var action = genericMethod.Invoke(delegateFactory, null);
                                                fakeParameters[index] = action;
                                            }
                                            else
                                            {
                                                fakeParameters[index] = delegateFactory.GetAction0();
                                            }
                                        }
                                        else if (ReflectionUtils.IsFuncType(parameterType))
                                        {
                                            var types = parameterType.GetGenericArguments();
                                            if (types.Length > 9)
                                                throw new NotSupportedException($"Func with less than 10 args supported '{parameter.Name}'");

                                            var methodName = $"GetFunc{types.Length}";

                                            var method = typeof(DefaultDelegateFactory).GetMethod(methodName);
                                            var genericMethod = method.MakeGenericMethod(types);
                                            var func = genericMethod.Invoke(delegateFactory, null);
                                            fakeParameters[index] = func;
                                        }
                                        else
                                            throw new NotSupportedException($"Unsupported delegate '{parameter.Name}' '{parameterType}' {Environment.StackTrace}");
                                    }
                                }
                                else
                                {
                                    fakeParameters[index] = CreateInstance(parameterType);
                                }
                            }
                            index++;
                        }
                        var instance = constructor.Invoke(fakeParameters);
                        return instance;
                    }
                }
            }
            catch (Exception ex)
            {
                //Debug.WriteLine($"Failed to create instance of type '{type.Name}'. Exception'{ex.Message}' '{ex}'");
            }
            return null;
        }

        private object CreateInstanceFast(Type type)
        {
            var instance = Activator.CreateInstance(type, nonPublicConstructors);
            return instance;
        }

        private IDictionary CloneDictionary(IDictionary items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IDictionary;

            foreach (DictionaryEntry entry in items)
            {
                var clonedKey = DoDeepClone(entry.Key);
                var clonedValue = DoDeepClone(entry.Value);

                result.Add(clonedKey, clonedValue);
            }
            return result;
        }

        private IDictionary<K, V> CloneGenericDictionary<K, V>(IDictionary<K, V> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IDictionary<K, V>;

            foreach (var entry in items)
            {
                var clonedKey = DoDeepClone(entry.Key);
                var clonedValue = DoDeepClone(entry.Value);

                result.Add((K)clonedKey, (V)clonedValue);
            }
            return result;
        }

        private IList CloneListOrCollection(IList items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IList;

            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.Add(clonedValue);
            }

            return result;
        }

        private IList<T> CloneGenericListOrCollection<T>(IList<T> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var result = CreateInstanceFast(type) as IList<T>;

            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.Add((T)clonedValue);
            }

            return result;
        }

        private Array CloneArray(Array items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            var type = items.GetType();
            var elementType = type.GetElementType();
            var result = Array.CreateInstance(elementType, items.Length);

            int index = 0;
            foreach (var item in items)
            {
                var clonedValue = DoDeepClone(item);
                result.SetValue(clonedValue, index);
                index++;
            }

            return result;
        }

        private object CloneObject(object originalInstance)
        {
            if (originalInstance == null)
                return null;

            var type = originalInstance.GetType();
            // circular
            var clonedInstanceRegistered = circularReferenceManager.TryGetInstance(originalInstance);
            if (clonedInstanceRegistered != null)
                return clonedInstanceRegistered;

            var clonedInstance = CreateInstance(type);
            if (clonedInstance != null)
            {
                // circular
                circularReferenceManager.AddInstance(originalInstance, clonedInstance);

                // fields
                if (includeFields)
                {
                    var fields = ReflectionUtils.GetFields(type, true);
                    foreach (var field in fields)
                    {
                        if (!blackList.Contains(field.Name))
                        {
                            var fieldValue = field.GetValue(originalInstance);
                            var clonedFieldValue = DoDeepClone(fieldValue);
                            field.SetValue(clonedInstance, clonedFieldValue);
                        }
                    }
                }

                // properties
                var properties = ReflectionUtils.GetProperties(type, NonPublicProperties);
                foreach (var property in properties)
                {
                    if (property.CanRead && property.CanWrite)
                    {
                        var propertyValue = property.GetValue(originalInstance);
                        if (propertyValue != null && !blackList.Contains(property.Name))
                        {
                            var clonedPropertyValue = DoDeepClone(propertyValue);
                            property.SetValue(clonedInstance, clonedPropertyValue);
                        }
                    }
                }

                return clonedInstance;
            }

            switch (errorHandling)
            {
                case ClonerErrorHandling.Continue:
                    //Debug.WriteLine($"Unable to clone \"{type.Name}\". Ignoring and continue cloning with \"ObjectNonClonableHandling.Continue\"");
                    break;
                case ClonerErrorHandling.Throw:
                    throw new NotSupportedException($"Unable to clone '{type.Name}' '{Environment.StackTrace}'");
                case ClonerErrorHandling.UseOriginalValue:
                    //Debug.WriteLine($"Unable to clone \"{type.Name}\". Ignoring and return original value with \"ObjectNonClonableHandling.UseOriginalValue\"");
                    return originalInstance;
            }

            return null;
        }

        private bool IsValueTypeExtended(Type type)
        {
            return type.IsValueType || type == typeof(string) || type == typeof(Uri);
        }

        private void ClearCircularReferences()
        {
            circularReferenceManager.Clear();
        }

        private object DoDeepClone(object value)
        {
            if (value == null)
                return null;

            var type = value.GetType();
            if (IsValueTypeExtended(type))
                return value;
            else if (ReflectionUtils.IsEnumerableType(type))
            {
                if (ReflectionUtils.IsGenericDictionaryType(type))
                {
                    // IDictionary<K,V>
                    var args = ReflectionUtils.GetGenericDictionaryArguments(type);
                    var genericMethod = cloneGenericDictionaryMethod.MakeGenericMethod(args[0], args[1]);
                    return genericMethod.Invoke(this, new object[] { value });
                }
                else if (ReflectionUtils.IsDictionaryType(type))
                {
                    // IDictionary
                    return CloneDictionary((IDictionary)value);
                }
                else if (type.IsArray)
                {
                    return CloneArray((Array)value);
                }
                else if (ReflectionUtils.IsGenericListOrCollectionType(type))
                {
                    // IList<T> or ICollection<T>
                    var args = ReflectionUtils.GetGenericListOrCollectionArguments(type);
                    var genericMethod = cloneGenericListOrCollectionMethod.MakeGenericMethod(args[0]);
                    return genericMethod.Invoke(this, new object[] { value });
                }
                else if (ReflectionUtils.IsListOrCollectionType(type))
                {
                    // IList or ICollection
                    return CloneListOrCollection((IList)value);
                }
            }
            else
            {
                if (value is Delegate)
                {
                    if (includeDelegates)
                        return CloneDelegate((Delegate)value);
                    else
                        return value;
                }
                else
                    return CloneObject(value);
            }
            throw new NotSupportedException($"Unable to clone type {value.GetType().Name}");
        }

        /// <summary>
        /// Clones the value.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value</param>
        /// <returns>The clone</returns>
        public T DeepClone<T>(T value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            ClearCircularReferences();

            return (T)DoDeepClone(value);
        }
    }

    /// <summary>
    /// The <see cref="Cloner"/> error handling.
    /// </summary>
    public enum ClonerErrorHandling
    {
        /// <summary>
        /// Continue.
        /// </summary>
        Continue,
        /// <summary>
        /// Throw a <see cref="NotSupportedException"/>.
        /// </summary>
        Throw,
        /// <summary>
        /// Use the original value.
        /// </summary>
        UseOriginalValue
    }
}
