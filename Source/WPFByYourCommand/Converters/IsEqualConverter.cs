using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class IsEqualConverter : IValueConverter
    {
        internal static bool GetValue(object value, object parameter)
        {
            bool result = false;
            if (value == null && parameter == null)
                result = true;
            else if (parameter == null)
                result = false;
            else if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    Int64 parameterValue;
                    if (parameter is string)
                        parameterValue = System.Convert.ToInt64(Enum.Parse(value.GetType(), parameter.ToString()), CultureInfo.InvariantCulture);
                    else
                        parameterValue = System.Convert.ToInt64(parameter, CultureInfo.InvariantCulture);
                    if (parameterValue == 0)
                        result = System.Convert.ToInt64(value, CultureInfo.InvariantCulture) == 0;
                    else
                        result = (System.Convert.ToInt64(value, CultureInfo.InvariantCulture) & parameterValue) == parameterValue;
                }
                else if (value is string)
                    result = value.ToString().Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);
                else
                    result = value == parameter;
            }
            return result;
        }


        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result = GetValue(value, parameter);

            if (targetType == typeof(object) || targetType == typeof(bool) || targetType == typeof(bool?))
                return result;
            if (targetType == typeof(int))
                return result ? 1 : 0;
            if (targetType == typeof(Visibility))
                return result ? Visibility.Visible : Visibility.Hidden;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
                return parameter;
            if (value is int && ((int)value) > 0)
                return parameter;
            if (value is Visibility && ((Visibility)value) == Visibility.Visible)
                return parameter;
            return null;
        }
    }

}
