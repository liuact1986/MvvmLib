using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace NavigationSample.Wpf.Converters
{
    public class DurationToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Duration)
            {
                return ConvertToString(value);
            }
            if (value is IEnumerable)
            {
                var enumerator = ((IEnumerable)value).GetEnumerator();
                var results = new List<string>();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is Duration)
                    {
                        var current = ConvertToString(enumerator.Current);
                        results.Add(current);
                    }
                }
                return results;
            }

            return value;
        }

        private string ConvertToString(object value)
        {
            string result = ((Duration)value).TimeSpan.TotalMilliseconds.ToString();
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if (double.TryParse(value.ToString(), out double r))
                {
                    return new Duration(TimeSpan.FromMilliseconds(r));
                }
            return value;
        }
    }

}
