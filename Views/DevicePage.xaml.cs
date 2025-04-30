// Namespace imports for UI controls, data binding, and model/service references
using System;                                    // Provides basic system types
using System.Linq;                               // Provides LINQ extension methods
using System.Threading.Tasks;                    // Provides Task-based async functionality
using Microsoft.Maui.Controls;                   // Provides MAUI UI elements
using SmartHomeApp.Models;                       // Provides Device model
using SmartHomeApp.Services;                     // Provides DatabaseService
using SmartHomeApp.ViewModels;                   // Provides MainViewModel
using DeviceModel = SmartHomeApp.Models.Device;  // Alias for Device model

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for DevicePage.xaml, handling user interactions for device management.
    /// </summary>
    public partial class DevicePage : ContentPage
    {
        // ViewModel instance providing device, group, and schedule data
        readonly MainViewModel _vm;

        /// <summary>
        /// Default constructor instantiating a new ViewModel when navigation lacks an existing instance.
        /// </summary>
        public DevicePage()
        {
            InitializeComponent();             // Load XAML components
            _vm = new MainViewModel();         // Create fresh ViewModel
            BindingContext = _vm;              // Assign data context for bindings
        }

        /// <summary>
        /// Constructor accepting an existing ViewModel instance for shared data context.
        /// </summary>
        /// <param name="vm">MainViewModel instance to bind to this page.</param>
        public DevicePage(MainViewModel vm)
        {
            InitializeComponent();             // Load XAML components
            _vm = vm;                          // Assign provided ViewModel
            BindingContext = _vm;              // Assign data context for bindings
        }

        /// <summary>
        /// Handles the Add Device button click event.
        /// Validates input, creates a new device entry, persists it, and updates the collection.
        /// </summary>
        private async void OnAddDeviceClicked(object sender, EventArgs e)
        {
            var name = DeviceNameEntry.Text?.Trim();                      
            var type = DeviceTypePicker.SelectedItem as string;           
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                return;  // Abort when name or type is missing

            var device = new DeviceModel
            {
                Name = name,
                Type = type,
                IsOn = false,
                Temperature = 75                           // Default thermostat setting
            };

            await DatabaseService.SaveDevice(device);  // Persist new device
            _vm.Devices.Add(device);                   // Update UI collection

            DeviceNameEntry.Text = string.Empty;      // Reset input field
            DeviceTypePicker.SelectedIndex = -1;       // Reset picker selection
        }

        /// <summary>
        /// Handles the Remove button click event for devices.
        /// Deletes the selected device from the database and updates the collection.
        /// </summary>
        private async void OnRemoveDeviceClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;  // Abort when CommandParameter is invalid

            var device = _vm.Devices.FirstOrDefault(d => d.Id == id);
            if (device == null)
                return;  // Abort when device not found

            await DatabaseService.DeleteDevice(device);  // Remove from database
            _vm.Devices.Remove(device);                 // Remove from UI collection
        }

        /// <summary>
        /// Handles the Set button click event for thermostat temperature adjustment.
        /// Animates temperature change stepwise, clamps input, and persists the final value.
        /// </summary>
        private async void OnSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                // Locate the adjacent Entry control within the layout
                if (btn.Parent is HorizontalStackLayout horiz &&
                    horiz.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
                    int.TryParse(entry.Text, out var target))
                {
                    target = Math.Clamp(target, 50, 90);  // Constrain temperature range

                    var old = dev.Temperature;
                    if (old != target)
                    {
                        var step = target > old ? 1 : -1;
                        // Animate temperature adjustment in increments
                        for (var t = old; t != target; t += step)
                        {
                            dev.Temperature = t + step;
                            await Task.Delay(200);          // Pause for animation effect
                        }
                    }

                    dev.Temperature = target;
                    entry.Text = target.ToString();      // Reflect final value in input
                    await DatabaseService.SaveDevice(dev);  // Persist updated thermostat setting
                }
            }
        }
    }
}
