using System;
using System.ComponentModel;
using System.Globalization;

namespace MvvmLib.Utils
{
    /// <summary>
    /// The type helper.
    /// </summary>
    public class TypeConversionHelper
    {
        private static readonly Type[] convertTypes = {
            typeof(Object),
            typeof(DBNull),
            typeof(Boolean),
            typeof(Char),
            typeof(SByte),
            typeof(Byte),
            typeof(Int16),
            typeof(UInt16),
            typeof(Int32),
            typeof(UInt32),
            typeof(Int64),
            typeof(UInt64),
            typeof(Single),
            typeof(Double),
            typeof(Decimal),
            typeof(DateTime),
            typeof(Object),
            typeof(String)
        };

        /// <summary>
        /// Converts the value to the property type if required.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <returns>The value or the value converted</returns>
        public static object TryConvert(Type type, object value, CultureInfo culture)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
            {
                return value;
            }

            if (CanConvertWithChangeType(type))
            {
                // first try with convert change type for numbers, boolean, datetime, char
                return ConvertWithChangeType(type, value, culture);
            }
            else
            {
                // Fails on enum, nullables, uri, timespan, brush, ... try with TypeConverter
                return ConvertWithTypeConverter(type, value, culture);
            }
        }

        /// <summary>
        /// Checks if Convert ChangeType can converts the value.
        /// </summary>
        /// <param name="type">The type</param>
        /// <returns>True if can convert</returns>
        public static bool CanConvertWithChangeType(Type type)
        {
            return Array.IndexOf(convertTypes, type) != -1;
        }

        /// <summary>
        /// Converts the value to the property type if required.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <returns>The value or the value converted</returns>
        public static object ConvertWithChangeType(Type type, object value, CultureInfo culture)
        {
            if (value == null)
            {
                return value;
            }

            var typeOfValue = value.GetType();
            if (!Equals(type, typeOfValue))
            {
                try
                {
                    if (culture != null)
                    {
                        var convertedValue = Convert.ChangeType(value, type, culture);
                        return convertedValue;
                    }
                    else
                    {
                        var convertedValue = Convert.ChangeType(value, type);
                        return convertedValue;
                    }
                }
                catch
                {
                    throw new NotSupportedException($"Unable to convert value '{value}' from type '{typeOfValue.Name}' to type '{type.Name}'");
                }
            }

            return value;
        }

        /// <summary>
        /// Converts the value to the property type if required.
        /// </summary>
        /// <typeparam name="TType">The type</typeparam>
        /// <typeparam name="TValue">The type of the value</typeparam>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <returns>The value or the value converted</returns>
        public static object TryConvert<TType, TValue>(TValue value, CultureInfo culture)
        {
            return TryConvert(typeof(TType), value, culture);
        }

        /// <summary>
        /// Converts the value to the property type with <see cref="TypeConverter"/> if required.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="value">The value</param>
        /// <param name="culture">The culture</param>
        /// <returns>The value or the value converted</returns>
        public static object ConvertWithTypeConverter(Type type, object value, CultureInfo culture)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (value == null)
            {
                return value;
            }

            var typeOfValue = value.GetType();
            var converter = TypeDescriptor.GetConverter(type);
            if (!converter.CanConvertFrom(typeOfValue))
                throw new NotSupportedException($"Unable to convert value '{value}' from type '{typeOfValue.Name}' to type '{type.Name}'");

            if (culture != null)
            {
                var convertedValue = converter.ConvertFrom(null, culture, value);
                return convertedValue;
            }
            else
            {
                var convertedValue = converter.ConvertFrom(value);
                return convertedValue;
            }
        }
    }
}
