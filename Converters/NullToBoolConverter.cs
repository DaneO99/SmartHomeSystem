using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SmartHomeApp.Converters
{
    public class NullToBoolConverter : IValueConverter
    {
        // must accept nullable object? parameters
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value is not null;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
