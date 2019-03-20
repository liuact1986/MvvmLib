using System;
using System.Collections;
using System.Collections.Generic;

namespace MvvmLib.IoC
{
    public class ValueContainer
    {
        protected Dictionary<string, object> values
          = new Dictionary<string, object>();

        public ValueContainer RegisterValue(string name, object value)
        {
            if (value == null)
            {
                this.values[name] = value;
            }
            else
            {
                if (IsTypeSupported(value.GetType()))
                {
                    this.values[name] = value;
                }
                else
                {
                    throw new Exception("Invalid type \"" + value.GetType().Name + "\". Only Value Types, string or array, enumerable, Uri supported");
                }
            }

            return this;
        }

        public static bool IsTypeSupported(Type type)
        {
            return type == typeof(string)
                || type.IsValueType
                || type.IsArray
                || typeof(IEnumerable).IsAssignableFrom(type)
                || type == typeof(Uri)
                || Nullable.GetUnderlyingType(type) != null;
        }

        public bool IsRegistered(string name)
        {
            return this.values.ContainsKey(name);
        }

        public object Get(string key)
        {
            return this.values[key];
        }

        public List<object> GetAllOfType(Type type)
        {
            var result = new List<object>();
            foreach (var value in values.Values)
            {
                if (value.GetType() == type)
                {
                    result.Add(value);
                }
            }
            return result;
        }

        public bool Unregister(string name)
        {
            if (this.IsRegistered(name))
            {
                this.values.Remove(name);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            this.values.Clear();
        }
    }
}