// Namespace imports for fundamental types, culture info, and MAUI bindings
using System;                     // Provides basic system types
using System.Globalization;       // Provides culture-specific information
using Microsoft.Maui.Controls;    // Contains IValueConverter interface for data binding

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Converter that determines visibility based on device type value.
    /// Returns true when input equals "Thermostat" (case-insensitive).
    /// </summary>
    public class ThermostatVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Evaluates the input string to check if it represents a thermostat device.
        /// </summary>
        /// <param name="value">Device type value as object; expected to be convertible to string.</param>
        /// <param name="targetType">Type expected by the binding target (unused).</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">Culture information for localization (unused).</param>
        /// <returns>
        /// True when <paramref name="value"/> equals "Thermostat" (ignoring case);
        /// otherwise false or null.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Convert value to string if not null and compare to "Thermostat"
            var text = value?.ToString();
            return text?.Equals("Thermostat", StringComparison.OrdinalIgnoreCase) == true;
        }

        /// <summary>
        /// Back conversion from visibility boolean to device type string is not supported.
        /// </summary>
        /// <param name="value">Value from binding target (unused).</param>
        /// <param name="targetType">Type expected by binding source (unused).</param>
        /// <param name="parameter">Optional parameter (unused).</param>
        /// <param name="culture">Culture information for localization (unused).</param>
        /// <returns>Never returns; always throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Indicates that back conversion is not implemented.</exception>
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Back conversion logic not required for this converter
            throw new NotImplementedException();
        }
    }
}
