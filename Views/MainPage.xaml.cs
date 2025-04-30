// Namespace imports for navigation, UI controls, and graphics utilities
using System;                                           
using Microsoft.Maui.Controls;                          // Provides ContentPage and navigation
using Microsoft.Maui.Graphics;                          // Provides Colors for UI element styling
using SmartHomeApp.ViewModels;                          // Provides MainViewModel for data binding

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for MainPage.xaml, managing navigation and quick-control actions.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        // Stores current armed/disarmed state of the alarm system
        bool _alarmArmed = false;

        /// <summary>
        /// Initializes XAML-defined UI components.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();  // Load controls and layout from XAML
        }

        /// <summary>
        /// Navigates to the device management page using existing ViewModel instance.
        /// </summary>
        private async void GoToDevices(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new DevicePage(vm));
        }

        /// <summary>
        /// Navigates to the room (group) management page using existing ViewModel instance.
        /// </summary>
        private async void GoToRooms(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new RoomPage(vm));
        }

        /// <summary>
        /// Navigates to the schedule management page using existing ViewModel instance.
        /// </summary>
        private async void GoToSchedules(object sender, EventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await Navigation.PushAsync(new SchedulePage(vm));
        }

        /// <summary>
        /// Toggles all lights on or off based on switch state.
        /// </summary>
        private async void OnLightsToggled(object sender, ToggledEventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await vm.ToggleAllOfType("Light", e.Value);
        }

        /// <summary>
        /// Toggles all door locks locked or unlocked based on switch state.
        /// </summary>
        private async void OnLocksToggled(object sender, ToggledEventArgs e)
        {
            var vm = (MainViewModel)BindingContext!;
            await vm.ToggleAllOfType("Door Lock", e.Value);
        }

        /// <summary>
        /// Arms or disarms the alarm system, updating button text and background color.
        /// </summary>
        private void OnAlarmButtonClicked(object sender, EventArgs e)
        {
            // Flip alarm armed state
            _alarmArmed = !_alarmArmed;
            
            // Update button label and color based on armed state
            AlarmButton.Text = _alarmArmed ? "Armed" : "Disarmed";
            AlarmButton.BackgroundColor = _alarmArmed ? Colors.Red : Colors.Blue;
        }
    }
}
