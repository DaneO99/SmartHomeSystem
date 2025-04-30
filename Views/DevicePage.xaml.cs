using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SmartHomeApp.Models;
using SmartHomeApp.Services;
using SmartHomeApp.ViewModels;
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Views
{
    public partial class DevicePage : ContentPage
    {
        readonly MainViewModel _vm;

        // Parameterless ctor (if you ever navigate here without a VM)
        public DevicePage()
        {
            InitializeComponent();
            _vm = new MainViewModel();
            BindingContext = _vm;
        }

        // This is the one MainPage needs
        public DevicePage(MainViewModel vm)
        {
            InitializeComponent();
            _vm = vm;
            BindingContext = _vm;
        }

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
            _vm.Devices.Add(device);

            DeviceNameEntry.Text = string.Empty;
            DeviceTypePicker.SelectedIndex = -1;
        }

        private async void OnRemoveDeviceClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;

            var device = _vm.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return;

            await DatabaseService.DeleteDevice(device);
            _vm.Devices.Remove(device);
        }

        private async void OnSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                if (btn.Parent is HorizontalStackLayout horiz &&
                    horiz.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
                    int.TryParse(entry.Text, out var target))
                {
                    target = Math.Clamp(target, 50, 90);

                    var old = dev.Temperature;
                    if (old != target)
                    {
                        var step = target > old ? 1 : -1;
                        for (var t = old; t != target; t += step)
                        {
                            dev.Temperature = t + step;
                            await Task.Delay(200);
                        }
                    }

                    dev.Temperature = target;
                    entry.Text = target.ToString();
                    await DatabaseService.SaveDevice(dev);
                }
            }
        }
    }
}
