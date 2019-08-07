using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFByYourCommand.Converters
{
    public class ProvideDefaultValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                value = Activator.CreateInstance(targetType);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
