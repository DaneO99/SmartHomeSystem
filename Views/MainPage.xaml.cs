using System;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SmartHomeApp.ViewModels;
using static SmartHomeApp.AppShell;

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
        }

        private async void GoToDevices(object sender, EventArgs e)
            => await Navigation.PushAsync(new DevicePage());

        private async void GoToRooms(object sender, EventArgs e)
            => await Navigation.PushAsync(new RoomPage());

        private async void GoToScheduleList(object sender, EventArgs e)
            => await Navigation.PushAsync(new ScheduleListPage());

        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            _alarmArmed = !_alarmArmed;
            AlarmButton.Text = _alarmArmed ? "Armed" : "Disarmed";
            AlarmButton.BackgroundColor = _alarmArmed ? Colors.Red : Colors.Blue;
        }

        private async void OnLightsToggledButton(object sender, EventArgs e)
        {
            bool anyOff = Vm.Devices.Any(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase) && !d.IsOn);
            await Vm.ToggleAllOfType("Light", anyOff);
            UpdateToggleStates();
        }

        private async void OnLocksToggledButton(object sender, EventArgs e)
        {
            bool anyOff = Vm.Devices.Any(d => d.Type.Equals("Door Lock", StringComparison.OrdinalIgnoreCase) && !d.IsOn);
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
            bool allLightsOn = Vm.Devices.Where(d => d.Type.Equals("Light", StringComparison.OrdinalIgnoreCase)).All(d => d.IsOn);
            LightsToggleButton.BackgroundColor = allLightsOn ? Colors.Blue : Colors.LightGray;

            bool allLocksOn = Vm.Devices.Where(d => d.Type.Equals("Door Lock", StringComparison.OrdinalIgnoreCase)).All(d => d.IsOn);
            LocksToggleButton.BackgroundColor = allLocksOn ? Colors.Blue : Colors.LightGray;
        }

        void UpdateCurrentTemp()
        {
            var thermo = Vm.Devices.FirstOrDefault(d => d.Type.Equals("Thermostat", StringComparison.OrdinalIgnoreCase));
            CurrentTempButton.Text = thermo != null ? $"Temp: {thermo.Temperature}°F" : "No Thermostat";
        }
    }
}
