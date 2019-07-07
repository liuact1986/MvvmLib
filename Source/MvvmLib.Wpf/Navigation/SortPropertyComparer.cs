using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace MvvmLib.Navigation
{
    /// <summary>
    /// A comparer that use Reflection and <see cref="SortDescriptionCollection"/> to sort.
    /// </summary>
    public class SortPropertyComparer : IComparer
    {
        private Comparer comparer;
        private SortPropertyInfo[] properties;

        /// <summary>
        /// Creates the <see cref="SortPropertyComparer"/>.
        /// </summary>
        /// <param name="sortDescriptions">The sort desciptions</param>
        /// <param name="culture">The culture</param>
        public SortPropertyComparer(SortDescriptionCollection sortDescriptions, CultureInfo culture)
        {
            if (sortDescriptions == null)
                throw new ArgumentNullException(nameof(sortDescriptions));

            this.properties = CreatePropertyInfo(sortDescriptions);
            this.comparer = GetComparer(culture);
        }

        private Comparer GetComparer(CultureInfo culture)
        {
            if (culture == null || culture == CultureInfo.InvariantCulture)
                return Comparer.DefaultInvariant;
            else if (culture == CultureInfo.CurrentCulture)
                return Comparer.Default;
            else
                return new Comparer(culture);
        }

        private SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortDescriptions)
        {
            var properties = new SortPropertyInfo[sortDescriptions.Count];
            for (int i = 0; i < sortDescriptions.Count; i++)
            {
                properties[i].PropertyName = sortDescriptions[i].PropertyName;
                properties[i].Descending = sortDescriptions[i].Direction == ListSortDirection.Descending;
            }
            return properties;
        }

        /// <summary>
        /// Compares the objects.
        /// </summary>
        /// <param name="o1">The first object</param>
        /// <param name="o2">The second object</param>
        /// <returns>The result of comparison</returns>
        public int Compare(object o1, object o2)
        {
            int result = 0;
            for (int i = 0; i < properties.Length; i++)
            {
                object v1 = properties[i].GetValue(o1);
                object v2 = properties[i].GetValue(o2);

                result = comparer.Compare(v1, v2);

                if (properties[i].Descending)
                    result = -result;

                if (result != 0)
                    break;
            }
            return result;
        }

        struct SortPropertyInfo
        {
            internal string PropertyName;
            internal bool Descending;
            internal PropertyInfo propertyCache;

            internal PropertyInfo GetProperty(object o)
            {
                if (propertyCache != null)
                    return propertyCache;
                else
                {
                    var property = o.GetType().GetProperty(PropertyName);
                    if (property == null)
                        throw new ArgumentException($"No property found for '{PropertyName}' in '{o.GetType().Name}'");
                    propertyCache = property;
                    return property;
                }
            }

            internal object GetValue(object o)
            {
                var property = GetProperty(o);
                var value = property.GetValue(o);
                return value;
            }
        }

    }

}
