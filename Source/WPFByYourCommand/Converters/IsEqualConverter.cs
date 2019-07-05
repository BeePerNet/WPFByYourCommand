using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class IsEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result = false;
            if (parameter.GetType().IsEnum)
                result = value.ToString() == parameter.ToString();
            else
                result = value == parameter;
            if (targetType == typeof(bool?))
                return result;
            if (targetType == typeof(Visibility))
                return result ? Visibility.Visible : Visibility.Hidden;
             return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return parameter;
            if (value is Visibility && ((Visibility)value) == Visibility.Visible)
                return parameter;
            return null;
        }
    }

}
