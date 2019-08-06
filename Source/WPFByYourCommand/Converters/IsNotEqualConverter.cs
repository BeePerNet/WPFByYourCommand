using System;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class IsNotEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result = !IsEqualConverter.GetValue(value, parameter);

            if (targetType == typeof(object) || targetType == typeof(bool) || targetType == typeof(bool?))
            {
                return result;
            }

            if (targetType == typeof(int))
            {
                return result ? 1 : 0;
            }

            if (targetType == typeof(Visibility))
            {
                return result ? Visibility.Visible : Visibility.Hidden;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
