using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class IsEqualConverter : BaseConverter, IValueConverter, IMultiValueConverter
    {
        internal static bool GetValue(object value, object parameter)
        {
            bool result = false;
            if (value == null && parameter == null)
            {
                result = true;
            }
            else if (parameter == null || value == null)
            {
                result = false;
            }
            else if (value is bool && !(parameter is bool))
            {
                if (bool.TryParse(parameter.ToString(), out bool boolvalue))
                {
                    result = (bool)value == boolvalue;
                }
            }
            else if (value.GetType().IsEnum)
            {
                long parameterValue;
                if (parameter is string)
                {
                    parameterValue = System.Convert.ToInt64(Enum.Parse(value.GetType(), parameter.ToString()), CultureInfo.InvariantCulture);
                }
                else
                {
                    parameterValue = System.Convert.ToInt64(parameter, CultureInfo.InvariantCulture);
                }

                if (parameterValue == 0)
                {
                    result = System.Convert.ToInt64(value, CultureInfo.InvariantCulture) == 0;
                }
                else
                {
                    result = (System.Convert.ToInt64(value, CultureInfo.InvariantCulture) & parameterValue) == parameterValue;
                }
            }
            else if (double.TryParse(value.ToString(), out double dValue) && double.TryParse(parameter.ToString(), out double dParam))
            {
                result = dValue == dParam;
            }
            else if (value is string)
            {
                result = value.ToString().Equals(parameter.ToString(), StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                result = value == parameter;
            }

            return result;
        }


        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result = GetValue(value, parameter);

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

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = values.Select(T => Convert(T, typeof(bool), parameter, culture)).Cast<bool>().Any();

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

            return values;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return parameter;
            }

            if (value is int && ((int)value) > 0)
            {
                return parameter;
            }

            if (value is Visibility && ((Visibility)value) == Visibility.Visible)
            {
                return parameter;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
