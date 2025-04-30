// Namespace imports for fundamental types, culture info, and MAUI binding interface
using System;                     // Provides basic system types
using System.Globalization;       // Provides culture-specific information
using Microsoft.Maui.Controls;    // Contains IValueConverter interface for data binding

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Evaluates equality between the bound value and converter parameter by comparing
    /// their string representations in a case-insensitive manner.
    /// Returns true when both match; otherwise false.
    /// </summary>
    public class TypeEqualsConverter : IValueConverter
    {
        /// <summary>
        /// Compares the string representation of the input value against the converter parameter.
        /// </summary>
        /// <param name="value">Object provided by the binding source; expected non-null for comparison.</param>
        /// <param name="targetType">Type expected by the binding target (unused).</param>
        /// <param name="parameter">Object supplied as converter parameter; expected non-null for comparison.</param>
        /// <param name="culture">Culture information for localization (unused).</param>
        /// <returns>
        /// True if both <paramref name="value"/> and <paramref name="parameter"/> convert to identical strings;
        /// false when either is null or the strings differ.
        /// </returns>
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Return false if either input or parameter is null
            if (value == null || parameter == null)
                return false;

            // Compare string representations case-insensitively
            return string.Equals(
                value.ToString(),
                parameter.ToString(),
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Back conversion from boolean to original type is not supported.
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
