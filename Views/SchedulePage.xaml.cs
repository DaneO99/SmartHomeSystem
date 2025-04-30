// Namespace imports for UI controls and view model references
using System;                                           // Provides basic system types
using Microsoft.Maui.Controls;                          // Provides MAUI UI elements and events
using DeviceModel = SmartHomeApp.Models.Device;          // Alias for Device model type
using SmartHomeApp.ViewModels;                          // Provides MainViewModel for data operations

namespace SmartHomeApp.Views
{
    /// <summary>
    /// Code-behind for SchedulePage.xaml, handling creation and removal of device schedules.
    /// </summary>
    public partial class SchedulePage : ContentPage
    {
        /// <summary>
        /// Reference to the shared main view model containing collections and commands.
        /// </summary>
        readonly MainViewModel _vm;

        /// <summary>
        /// Initializes UI components and assigns the provided view model as the binding context.
        /// </summary>
        /// <param name="vm">Instance of MainViewModel shared across pages.</param>
        public SchedulePage(MainViewModel vm)
        {
            InitializeComponent();             // Load XAML-defined layout and controls
            BindingContext = _vm = vm;         // Assign view model for data binding
        }

        /// <summary>
        /// Handles the Save Schedule button click event.
        /// Creates and persists an on/off schedule for the selected device.
        /// </summary>
        /// <param name="sender">Button that triggered the event.</param>
        /// <param name="e">Event arguments (unused).</param>
        async void OnSaveScheduleClicked(object sender, EventArgs e)
        {
            // Ensure a valid device selection before scheduling
            if (DevicePicker.SelectedItem is DeviceModel device)
            {
                await _vm.AddSchedule(
                    device,
                    OnTimePicker.Time,
                    OffTimePicker.Time);
            }
        }

        /// <summary>
        /// Handles the Remove Schedule button click event.
        /// Deletes the schedule with the specified identifier.
        /// </summary>
        /// <param name="sender">Button that triggered the event.</param>
        /// <param name="e">Event arguments (unused).</param>
        async void OnRemoveScheduleClicked(object sender, EventArgs e)
        {
            // Extract schedule identifier from the button's CommandParameter
            if ((sender as Button)?.CommandParameter is int id)
            {
                await _vm.RemoveSchedule(id);
            }
        }
    }
}
