using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Returns true when the bound value’s .ToString() matches the converter parameter’s .ToString().
    /// </summary>
    public class TypeEqualsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return string.Equals(
                value.ToString(),
                parameter.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
