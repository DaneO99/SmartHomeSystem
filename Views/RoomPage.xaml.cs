using System;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.Services;
using SmartHomeApp.ViewModels;
using DeviceModel = SmartHomeApp.Models.Device;
using GroupModel  = SmartHomeApp.Models.DeviceGroup;

namespace SmartHomeApp.Views
{
    public partial class RoomPage : ContentPage
    {
        readonly MainViewModel _vm;

        public RoomPage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
            RefreshGroupDeviceAssignments();
        }

        private async void OnAddRoomClicked(object sender, EventArgs e)
        {
            var name = NewRoomEntry.Text?.Trim();
            if (string.IsNullOrEmpty(name)) return;

            var grp = new GroupModel { GroupName = name };
            await DatabaseService.SaveGroup(grp);
            _vm.Groups.Add(grp);
            NewRoomEntry.Text = "";
            RefreshGroupDeviceAssignments();
        }

        private async void OnRemoveRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            var grp = _vm.Groups.FirstOrDefault(g => g.Id == id);
            if (grp == null) return;

            // unassign all devices in this room
            foreach (var d in _vm.Devices.Where(d => d.GroupId == grp.Id).ToList())
            {
                d.GroupId = null!;
                await DatabaseService.SaveDevice(d);
            }

            await DatabaseService.DeleteGroup(grp);
            _vm.Groups.Remove(grp);
            RefreshGroupDeviceAssignments();
        }

        private async void OnAssignDeviceClicked(object sender, EventArgs e)
        {
            if (DevicePicker.SelectedItem is not DeviceModel dev ||
                RoomPicker.SelectedItem  is not GroupModel  grp)
                return;

            dev.GroupId = grp.Id;
            await DatabaseService.SaveDevice(dev);
            RefreshGroupDeviceAssignments();
        }

        private async void OnRemoveDeviceFromRoomClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            var dev = _vm.Devices.FirstOrDefault(d => d.Id == id);
            if (dev == null) return;

            dev.GroupId = null!;
            await DatabaseService.SaveDevice(dev);
            RefreshGroupDeviceAssignments();
        }

        private async void OnRoomSetTempClicked(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is DeviceModel dev)
            {
                if (btn.Parent is HorizontalStackLayout h &&
                    h.Children.OfType<Entry>().FirstOrDefault() is Entry entry &&
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

        void RefreshGroupDeviceAssignments()
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
