using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.Services;
using SmartHomeApp.Views; // for AppShell

namespace SmartHomeApp.Converters
{
    /// <summary>
    /// Converts a comma-separated list of device IDs into a
    /// comma-separated list of device names, looking them up
    /// from the shared MainViewModel.Devices collection.
    /// </summary>
    public class DeviceIdsToNamesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var csv = value as string ?? "";
            var ids = csv
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out var i) ? i : (int?)null)
                .Where(i => i.HasValue)
                .Select(i => i.Value)
                .ToList();

            var deviceNames = AppShell.AppViewModel.Devices
                .Where(d => ids.Contains(d.Id))
                .Select(d => d.Name);

            return string.Join(", ", deviceNames);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
