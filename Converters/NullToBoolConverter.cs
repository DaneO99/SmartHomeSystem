// Namespace imports for fundamental types, culture info, and MAUI bindings
using System;                     // Provides basic system types
using System.Globalization;       // Provides culture-specific information
using Microsoft.Maui.Controls;    // Contains IValueConverter interface for data binding

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Converter that transforms an object reference into a boolean indicating
    /// whether the reference is non-null.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Evaluates the input object and returns a boolean reflecting its null status.
        /// </summary>
        /// <param name="value">Object to evaluate for null.</param>
        /// <param name="targetType">Type expected by the binding target (unused).</param>
        /// <param name="parameter">Optional parameter passed to the converter (unused).</param>
        /// <param name="culture">Culture information for localization (unused).</param>
        /// <returns>
        /// True when <paramref name="value"/> is not null; false when null.
        /// </returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Return true if the provided value is not null, otherwise false
            return value is not null;
        }

        /// <summary>
        /// Back conversion from boolean to object is not supported.
        /// </summary>
        /// <param name="value">Value coming from the binding target (unused).</param>
        /// <param name="targetType">Type expected by the binding source (unused).</param>
        /// <param name="parameter">Optional parameter passed to the converter (unused).</param>
        /// <param name="culture">Culture information for localization (unused).</param>
        /// <returns>Never returns; always throws <see cref="NotImplementedException"/>.</returns>
        /// <exception cref="NotImplementedException">Indicates that back conversion is not implemented.</exception>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // Back conversion logic not required for this converter
            throw new NotImplementedException();
        }
    }
}
