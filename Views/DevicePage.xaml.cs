using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SmartHomeApp.Services;
using SmartHomeApp.ViewModels;       // For MainViewModel
using static SmartHomeApp.AppShell;  // For AppViewModel
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code‐behind for DevicePage.xaml.
    /// Implements add/remove and animated thermostat updates.
    /// </summary>
    public partial class DevicePage : ContentPage
    {
        // Shortcut to the shared ViewModel
        MainViewModel Vm => AppViewModel;

        public DevicePage()
        {
            InitializeComponent();
            BindingContext = Vm;
        }

        /// <summary>
        /// Handler for the "+" button: adds a new device.
        /// </summary>
        private async void OnAddDeviceClicked(object sender, EventArgs e)
        {
            var name = DeviceNameEntry.Text?.Trim();
            var type = DeviceTypePicker.SelectedItem as string;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                return;

            var device = new DeviceModel
            {
                Name = name,
                Type = type,
                IsOn = false,
                Temperature = 75
            };

            await DatabaseService.SaveDevice(device);
            Vm.Devices.Add(device);

            DeviceNameEntry.Text = "";
            DeviceTypePicker.SelectedIndex = -1;
        }

        /// <summary>
        /// Handler for the red "✕" button: removes the device.
        /// </summary>
        private async void OnRemoveDeviceClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;

            var dev = Vm.Devices.FirstOrDefault(d => d.Id == id);
            if (dev == null) return;

            await DatabaseService.DeleteDevice(dev);
            Vm.Devices.Remove(dev);
        }

        /// <summary>
        /// Handler for the "Set" button in the thermostat expander:
        /// animates the label from current to target one degree at a time.
        /// </summary>
        private async void OnSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                // Find the Entry next to this button
                if (btn.Parent is HorizontalStackLayout layout &&
                    layout.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
                    int.TryParse(entry.Text, out var rawTarget))
                {
                    // Clamp user input to [60, 90]
                    var target = Math.Clamp(rawTarget, 60, 90);

                    // Animate label (bound to dev.Temperature)
                    var current = dev.Temperature;
                    if (current != target)
                    {
                        var step = target > current ? 1 : -1;
                        for (var t = current; t != target; t += step)
                        {
                            dev.Temperature = t + step;
                            await Task.Delay(200);  // pause for counter effect
                        }
                    }

                    // Persist the final temperature
                    await DatabaseService.SaveDevice(dev);
                }
            }
        }
    }
}
