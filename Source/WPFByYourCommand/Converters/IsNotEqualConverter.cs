﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class IsNotEqualConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            bool result = false;
            if (value == null && parameter == null)
                result = true;
            else if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    Int64 parameterValue = 0;
                    if (parameter is string)
                        parameterValue = System.Convert.ToInt64(Enum.Parse(value.GetType(), parameter.ToString()));
                    else
                        parameterValue = System.Convert.ToInt64(parameter);
                    if (parameterValue == 0)
                        result = System.Convert.ToInt64(value) == 0;
                    else
                        result = (System.Convert.ToInt64(value) & parameterValue) == parameterValue;
                }
                else
                    result = value == parameter;
            }
            result = !result;
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
            throw new NotSupportedException();
        }
    }

}
