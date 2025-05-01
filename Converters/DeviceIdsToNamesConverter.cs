using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.ViewModels;    // for AppViewModel alias
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Converts a comma-separated list of device IDs into a comma-separated list of device Names.
    /// </summary>
    public class DeviceIdsToNamesConverter : IValueConverter
    {
        // Convert from IDs csv → Names csv
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var csv = value as string;
            if (string.IsNullOrWhiteSpace(csv))
                return string.Empty;

            // parse ints, skipping any that fail
            var ids = csv
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var i) ? (int?)i : null)
                .Where(i => i.HasValue)
                .Select(i => i!.Value)
                .ToList();

            // look up names in the shared ViewModel
            var names = AppShell.AppViewModel.Devices
                .Where(d => ids.Contains(d.Id))
                .Select(d => d.Name);

            return string.Join(", ", names);
        }

        // Not needed for one-way binding
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
