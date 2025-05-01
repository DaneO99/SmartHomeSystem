using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;      // For ContentPage, Button, Entry, etc.
using SmartHomeApp.Services;        // For DatabaseService
using SmartHomeApp.ViewModels;      // For MainViewModel
using static SmartHomeApp.AppShell; // For AppShell.AppViewModel
using DeviceModel = SmartHomeApp.Models.Device;          // Alias model types
using GroupModel  = SmartHomeApp.Models.DeviceGroup;     // Alias model types

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for RoomPage.xaml. 
    /// Binds to the single shared MainViewModel (AppShell.AppViewModel).
    /// </summary>
    public partial class RoomPage : ContentPage
    {
        /// <summary>
        /// Shared ViewModel instance from AppShell.
        /// </summary>
        MainViewModel Vm => AppViewModel;

        public RoomPage()
        {
            InitializeComponent();
            BindingContext = Vm;                  // Inherit the shared VM
            RefreshGroupDeviceAssignments();     // Populate DevicesInGroup collections
        }

        /// <summary>
        /// Adds a new room with the entered name.
        /// Persists it and updates the UI.
        /// </summary>
        private async void OnAddRoomClicked(object sender, EventArgs e)
        {
            var name = NewRoomEntry.Text?.Trim();
            if (string.IsNullOrEmpty(name))
                return;

            var grp = new GroupModel { GroupName = name };
            await DatabaseService.SaveGroup(grp);
            Vm.Groups.Add(grp);

            NewRoomEntry.Text = string.Empty;
            RefreshGroupDeviceAssignments();
        }

        /// <summary>
        /// Removes the room (and unassigns its devices) given by CommandParameter (Id).
        /// </summary>
        private async void OnRemoveRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;

            var grp = Vm.Groups.FirstOrDefault(g => g.Id == id);
            if (grp == null)
                return;

            // Unassign devices from this room
            foreach (var d in Vm.Devices.Where(d => d.GroupId == grp.Id).ToList())
            {
                d.GroupId = null;
                await DatabaseService.SaveDevice(d);
            }

            // Delete the room and update UI
            await DatabaseService.DeleteGroup(grp);
            Vm.Groups.Remove(grp);
            RefreshGroupDeviceAssignments();
        }

        /// <summary>
        /// Assigns the selected device to the selected room.
        /// </summary>
        private async void OnAssignDeviceClicked(object sender, EventArgs e)
        {
            if (DevicePicker.SelectedItem is not DeviceModel dev ||
                RoomPicker.SelectedItem   is not GroupModel grp)
                return;

            dev.GroupId = grp.Id;
            await DatabaseService.SaveDevice(dev);

            RefreshGroupDeviceAssignments();
        }

        /// <summary>
        /// Removes the device from its room given by CommandParameter (device Id).
        /// </summary>
        private async void OnRemoveDeviceFromRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id)
                return;

            var dev = Vm.Devices.FirstOrDefault(d => d.Id == id);
            if (dev == null)
                return;

            dev.GroupId = null;
            await DatabaseService.SaveDevice(dev);

            RefreshGroupDeviceAssignments();
        }

        /// <summary>
        /// Animates and saves a new temperature for a thermostat device in the room.
        /// </summary>
        private async void OnRoomSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                // Find the sibling Entry inside the same horizontal layout
                if (btn.Parent is HorizontalStackLayout layout &&
                    layout.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
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
                            await Task.Delay(200); // brief animation delay
                        }
                    }

                    dev.Temperature = target;
                    entry.Text = target.ToString();
                    await DatabaseService.SaveDevice(dev);
                }
            }
        }

        /// <summary>
        /// Updates each DeviceGroup.DevicesInGroup based on Device.GroupId.
        /// </summary>
        private void RefreshGroupDeviceAssignments()
        {
            foreach (var grp in Vm.Groups)
            {
                grp.DevicesInGroup.Clear();
                foreach (var d in Vm.Devices.Where(d => d.GroupId == grp.Id))
                {
                    grp.DevicesInGroup.Add(d);
                }
            }
        }
    }
}
