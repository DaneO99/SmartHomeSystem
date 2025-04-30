using System;
using Microsoft.Maui.Controls;
using DeviceModel = SmartHomeApp.Models.Device;
using SmartHomeApp.ViewModels;

namespace SmartHomeApp.Views
{
    public partial class SchedulePage : ContentPage
    {
        readonly MainViewModel _vm;

        public SchedulePage(MainViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        async void OnSaveScheduleClicked(object sender, EventArgs e)
        {
            // disambiguate DeviceModel vs Maui.Controls.Device
            if (DevicePicker.SelectedItem is DeviceModel device)
            {
                // create an on/off schedule
                await _vm.AddSchedule(device,
                                      OnTimePicker.Time,
                                      OffTimePicker.Time);
            }
        }

        async void OnRemoveScheduleClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is int id)
            {
                await _vm.RemoveSchedule(id);
            }
        }
    }
}
