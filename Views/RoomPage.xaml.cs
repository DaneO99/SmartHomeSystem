// Imports for UI controls, navigation, and LINQ utilities
using System;                                       // Provides basic system types
using System.Linq;                                  // Provides LINQ extension methods
using Microsoft.Maui.Controls;                      // Provides ContentPage and event args
using SmartHomeApp.Services;                        // Provides DatabaseService for persistence
using SmartHomeApp.ViewModels;                      // Provides MainViewModel for data binding
using DeviceModel = SmartHomeApp.Models.Device;     // Alias for Device model type
using GroupModel  = SmartHomeApp.Models.DeviceGroup;// Alias for DeviceGroup model type

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for RoomPage.xaml, handling room creation, deletion,
    /// device assignment, and in-room device controls.
    /// </summary>
    public partial class RoomPage : ContentPage
    {
        /// <summary>
        /// Shared view model instance for accessing device, group, and schedule collections.
        /// </summary>
        readonly MainViewModel _vm;

        /// <summary>
        /// Initializes UI components, assigns binding context, and populates device lists for each group.
        /// </summary>
        /// <param name="vm">MainViewModel instance for data operations.</param>
        public RoomPage(MainViewModel vm)
        {
            InitializeComponent();                 // Load XAML-defined layout
            BindingContext = _vm = vm;             // Assign shared view model for bindings
            RefreshGroupDeviceAssignments();       // Populate DevicesInGroup for each group
        }

        /// <summary>
        /// Handles the Add Room button click event.
        /// Creates a new group, persists it, and updates the UI and assignments.
        /// </summary>
        private async void OnAddRoomClicked(object sender, EventArgs e)
        {
            var name = NewRoomEntry.Text?.Trim(); // Retrieve and trim new room name
            if (string.IsNullOrEmpty(name))
                return;                           // Abort when name is missing

            var grp = new GroupModel { GroupName = name };
            await DatabaseService.SaveGroup(grp);  // Persist new group
            _vm.Groups.Add(grp);                  // Update group collection bound to UI
            NewRoomEntry.Text = string.Empty;     // Clear input field
            RefreshGroupDeviceAssignments();      // Refresh device lists per group
        }

        /// <summary>
        /// Handles the Remove Room button click event.
        /// Unassigns devices from the removed room, deletes the group, and updates UI.
        /// </summary>
        private async void OnRemoveRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;                           // Abort when parameter is invalid

            var grp = _vm.Groups.FirstOrDefault(g => g.Id == id);
            if (grp == null)
                return;                           // Abort when group not found

            // Unassign devices currently in this room
            foreach (var d in _vm.Devices.Where(d => d.GroupId == grp.Id).ToList())
            {
                d.GroupId = null;                  // Clear group association
                await DatabaseService.SaveDevice(d); // Persist device update
            }

            await DatabaseService.DeleteGroup(grp); // Remove group from database
            _vm.Groups.Remove(grp);                // Update group collection
            RefreshGroupDeviceAssignments();       // Refresh device lists per group
        }

        /// <summary>
        /// Handles the Assign Device button click event.
        /// Assigns selected device to selected room and updates assignments.
        /// </summary>
        private async void OnAssignDeviceClicked(object sender, EventArgs e)
        {
            if (DevicePicker.SelectedItem is not DeviceModel dev ||
                RoomPicker.SelectedItem   is not GroupModel  grp)
                return;                           // Abort when selection is invalid

            dev.GroupId = grp.Id;                  // Set device's group association
            await DatabaseService.SaveDevice(dev); // Persist device update
            RefreshGroupDeviceAssignments();       // Refresh device lists per group
        }

        /// <summary>
        /// Handles the Remove Device button click event within a room.
        /// Clears group association for the specified device and updates assignments.
        /// </summary>
        private async void OnRemoveDeviceFromRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;                           // Abort when parameter is invalid

            var dev = _vm.Devices.FirstOrDefault(d => d.Id == id);
            if (dev == null)
                return;                           // Abort when device not found

            dev.GroupId = null;                   // Clear group association
            await DatabaseService.SaveDevice(dev); // Persist device update
            RefreshGroupDeviceAssignments();      // Refresh device lists per group
        }

        /// <summary>
        /// Handles the Set Temperature button click event for thermostat devices within a room.
        /// Animates temperature change and persists the final value.
        /// </summary>
        private async void OnRoomSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                if (btn.Parent is HorizontalStackLayout h &&
                    h.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
                    int.TryParse(entry.Text, out var target))
                {
                    target = Math.Clamp(target, 50, 90); // Constrain target temperature
                    var old = dev.Temperature;

                    if (old != target)
                    {
                        var step = target > old ? 1 : -1;
                        // Animate incremental temperature change
                        for (var t = old; t != target; t += step)
                        {
                            dev.Temperature = t + step;
                            await Task.Delay(200);
                        }
                    }

                    dev.Temperature = target;       // Ensure final value assignment
                    entry.Text = target.ToString(); // Reflect final value in UI
                    await DatabaseService.SaveDevice(dev); // Persist device update
                }
            }
        }

        /// <summary>
        /// Updates DevicesInGroup collection for each group based on current device associations.
        /// </summary>
        private void RefreshGroupDeviceAssignments()
        {
            foreach (var grp in _vm.Groups)
            {
                grp.DevicesInGroup.Clear();
                foreach (var d in _vm.Devices.Where(d => d.GroupId == grp.Id))
                    grp.DevicesInGroup.Add(d);
            }
        }
    }
}
