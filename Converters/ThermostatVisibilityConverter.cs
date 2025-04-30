using System;
using System.Globalization;
using Microsoft.Maui.Controls;  // for IValueConverter

namespace SmartHomeApp.Converters
{
    public class ThermostatVisibilityConverter : IValueConverter
    {
        // Note all reference‐type parameters and return are nullable (object?).
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Only visible if device type equals "Thermostat"
            return value?.ToString()
                   ?.Equals("Thermostat", StringComparison.OrdinalIgnoreCase)
                   == true;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
