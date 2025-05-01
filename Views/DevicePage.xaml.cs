using System;
using System.Linq;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using SmartHomeApp.ViewModels;
using SmartHomeApp.Services;
using DeviceModel = SmartHomeApp.Models.Device;
using static SmartHomeApp.AppShell;

namespace SmartHomeApp.Views
{
    public partial class DevicePage : ContentPage
    {
        MainViewModel Vm => AppViewModel;

        public DevicePage()
        {
            InitializeComponent();
            BindingContext = Vm;

            // Initial population
            SetupLists();

            // Update on any change
            Vm.Devices.CollectionChanged += OnDevicesChanged;
        }

        void OnDevicesChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => SetupLists();

        void SetupLists()
        {
            // Filter by Type
            LightsList.ItemsSource      = Vm.Devices.Where(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase));
            LocksList.ItemsSource       = Vm.Devices.Where(d => d.Type.Equals("Door Lock", StringComparison.OrdinalIgnoreCase));
            ThermostatList.ItemsSource  = Vm.Devices.Where(d => d.Type.Equals("Thermostat", StringComparison.OrdinalIgnoreCase));
        }

        private async void OnAddDeviceClicked(object sender, EventArgs e)
        {
            var name = DeviceNameEntry.Text?.Trim();
            var type = DeviceTypePicker.SelectedItem as string;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                return;

            var device = new DeviceModel
            {
                Name        = name,
                Type        = type,
                IsOn        = false,
                Temperature = 75
            };

            await DatabaseService.SaveDevice(device);
            Vm.Devices.Add(device);
            SetupLists();

            DeviceNameEntry.Text      = string.Empty;
            DeviceTypePicker.SelectedIndex = -1;
        }

        private async void OnRemoveDeviceClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;

            var device = Vm.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null) return;

            await DatabaseService.DeleteDevice(device);
            Vm.Devices.Remove(device);
            SetupLists();
        }

        private async void OnSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                if (btn.Parent is HorizontalStackLayout h
                    && h.Children.OfType<Entry>().FirstOrDefault() is Entry entry
                    && int.TryParse(entry.Text, out var target))
                {
                    // Clamp
                    target = Math.Clamp(target, 50, 90);

                    // Animate stepping
                    var old = dev.Temperature;
                    var step = target > old ? 1 : -1;
                    for (var t = old; t != target; t += step)
                    {
                        dev.Temperature = t + step;
                        await Task.Delay(50);
                    }

                    // Persist
                    await DatabaseService.SaveDevice(dev);
                }
            }
        }
    }
}
