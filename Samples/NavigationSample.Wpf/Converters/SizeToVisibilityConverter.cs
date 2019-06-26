using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NavigationSample.Wpf.Converters
{

    public class SizeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (int.TryParse(value.ToString(), out int size))
                {
                    if (size > 0)
                        return Visibility.Visible;
                    else
                        return Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
