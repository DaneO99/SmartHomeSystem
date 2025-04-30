using System;
using Microsoft.Maui.Graphics;      // for Colors.Red / Colors.Blue
using SmartHomeApp.ViewModels;

namespace SmartHomeApp.Views
{
    public partial class MainPage : ContentPage
    {
        // track armed state
        bool _alarmArmed = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void GoToDevices(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new DevicePage(vm));
        }

        private async void GoToRooms(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new RoomPage(vm));
        }

        private async void GoToSchedules(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new SchedulePage(vm));
        }

        private async void OnLightsToggled(object sender, ToggledEventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await vm.ToggleAllOfType("Light", e.Value);
        }

        private async void OnLocksToggled(object sender, ToggledEventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await vm.ToggleAllOfType("Door Lock", e.Value);
        }

        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            // flip state
            _alarmArmed = !_alarmArmed;

            // update UI
            AlarmButton.Text = _alarmArmed ? "Armed" : "Disarmed";
            AlarmButton.BackgroundColor = _alarmArmed ? Colors.Red : Colors.Blue;
        }
    }
}
