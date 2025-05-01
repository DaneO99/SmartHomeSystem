using System;
using System.Linq;
using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.ApplicationModel;
using SmartHomeApp.ViewModels;
using static SmartHomeApp.AppShell;
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Views
{
    public partial class MainPage : ContentPage
    {
        bool _alarmArmed = false;
        MainViewModel Vm => AppViewModel;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = Vm;

            Vm.Devices.CollectionChanged += Devices_CollectionChanged;
            foreach (var device in Vm.Devices)
                SubscribeToDevice(device);

            UpdateToggleStates();
            UpdateCurrentTemp();
        }

        void Devices_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (DeviceModel device in e.NewItems)
                    SubscribeToDevice(device);

            if (e.OldItems != null)
                foreach (DeviceModel device in e.OldItems)
                    device.PropertyChanged -= Device_PropertyChanged;

            MainThread.BeginInvokeOnMainThread(UpdateCurrentTemp);
        }

        void SubscribeToDevice(DeviceModel device)
            => device.PropertyChanged += Device_PropertyChanged;

        void Device_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DeviceModel.Temperature)
                && sender is DeviceModel dev
                && dev.Type.Equals("Thermostat", StringComparison.OrdinalIgnoreCase))
            {
                MainThread.BeginInvokeOnMainThread(UpdateCurrentTemp);
            }
        }

        private async void GoToDevices(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//devices");

        private async void GoToRooms(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//rooms");

        private async void GoToSchedules(object sender, EventArgs e)
            => await Shell.Current.GoToAsync("//schedules");

        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            _alarmArmed = !_alarmArmed;
            AlarmButton.Text = _alarmArmed ? "Armed" : "Disarmed";
            AlarmButton.BackgroundColor = _alarmArmed ? Colors.Red : Colors.Blue;
        }

        private async void OnLightsToggledButton(object sender, EventArgs e)
        {
            bool anyOff = Vm.Devices
                .Any(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase) && !d.IsOn);
            await Vm.ToggleAllOfType("Light", anyOff);
            UpdateToggleStates();
        }

        private async void OnLocksToggledButton(object sender, EventArgs e)
        {
            bool anyOff = Vm.Devices
                .Any(d => d.Type.Equals("Door Lock", StringComparison.OrdinalIgnoreCase) && !d.IsOn);
            await Vm.ToggleAllOfType("Door Lock", anyOff);
            UpdateToggleStates();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateToggleStates();
            UpdateCurrentTemp();
        }

        void UpdateToggleStates()
        {
            bool allLightsOn = Vm.Devices
                .Where(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase))
                .All(d => d.IsOn);
            LightsToggleButton.BackgroundColor = allLightsOn ? Colors.Blue : Colors.LightGray;

            bool allLocksOn = Vm.Devices
                .Where(d => d.Type.Equals("Door Lock", StringComparison.OrdinalIgnoreCase))
                .All(d => d.IsOn);
            LocksToggleButton.BackgroundColor = allLocksOn ? Colors.Blue : Colors.LightGray;
        }

        void UpdateCurrentTemp()
        {
            var thermostat = Vm.Devices
                .FirstOrDefault(d => d.Type.Equals("Thermostat", StringComparison.OrdinalIgnoreCase));
            CurrentTempButton.Text = thermostat != null
                ? $"Temp: {thermostat.Temperature}°F"
                : "No Thermostat";
        }
    }
}
