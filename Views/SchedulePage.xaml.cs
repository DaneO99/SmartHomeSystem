using System;
using System.Collections.Generic;
using System.Linq;
using SmartHomeApp.Models;
using SmartHomeApp.Services;
using SmartHomeApp.ViewModels;
using Microsoft.Maui.Controls;
using static SmartHomeApp.AppShell;
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Views
{
    public partial class SchedulePage : ContentPage
    {
        MainViewModel Vm => AppViewModel;

        // Selected device IDs
        readonly HashSet<int> _selectedDeviceIds = new();

        // Editing state
        private int? _editingScheduleId;

        public SchedulePage()
        {
            InitializeComponent();
            BindingContext = Vm;
        }

        void OnDeviceCheckChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox cb && cb.BindingContext is DeviceModel dev)
            {
                if (e.Value) _selectedDeviceIds.Add(dev.Id);
                else _selectedDeviceIds.Remove(dev.Id);
            }
        }

        private void OnCancelEditClicked(object sender, EventArgs e)
            => ExitEditMode();

        private void OnEditScheduleClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            var sched = Vm.Schedules.FirstOrDefault(s => s.Id == id);
            if (sched == null) return;

            _editingScheduleId = id;
            FormTitle.Text = "Edit Schedule";
            ScheduleNameEntry.Text = sched.Name;

            _selectedDeviceIds.Clear();
            foreach (var d in sched.DeviceIds) _selectedDeviceIds.Add(d);
            foreach (var child in DeviceSelectionLayout.Children.OfType<HorizontalStackLayout>())
            {
                if (child.Children[0] is CheckBox cb && cb.BindingContext is DeviceModel dev)
                    cb.IsChecked = _selectedDeviceIds.Contains(dev.Id);
            }

            OnTimePicker.Time      = sched.OnTime;
            OffTimePicker.Time     = sched.OffTime;
            DesiredTempEntry.Text  = sched.DesiredTemp?.ToString() ?? "";

            SundayCheck.IsChecked    = sched.Sunday;
            MondayCheck.IsChecked    = sched.Monday;
            TuesdayCheck.IsChecked   = sched.Tuesday;
            WednesdayCheck.IsChecked = sched.Wednesday;
            ThursdayCheck.IsChecked  = sched.Thursday;
            FridayCheck.IsChecked    = sched.Friday;
            SaturdayCheck.IsChecked  = sched.Saturday;

            SaveScheduleButton.IsVisible   = false;
            UpdateScheduleButton.IsVisible = true;
            CancelEditButton.IsVisible     = true;
        }

        private async void OnSaveScheduleClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ScheduleNameEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a schedule name.", "OK");
                return;
            }
            var sched = BuildScheduleFromForm();
            if (sched == null) return;

            await DatabaseService.SaveSchedule(sched);
            Vm.Schedules.Add(sched);
            ResetForm();
        }

        private async void OnUpdateScheduleClicked(object sender, EventArgs e)
        {
            if (!_editingScheduleId.HasValue) return;
            if (string.IsNullOrWhiteSpace(ScheduleNameEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a schedule name.", "OK");
                return;
            }

            var original = Vm.Schedules.FirstOrDefault(s => s.Id == _editingScheduleId.Value);
            if (original == null) return;

            var updated = BuildScheduleFromForm(original);
            if (updated == null) return;

            await DatabaseService.SaveSchedule(updated);
            var idx = Vm.Schedules.IndexOf(original);
            Vm.Schedules[idx] = updated;
            ExitEditMode();
        }

        private DeviceSchedule? BuildScheduleFromForm(DeviceSchedule? original = null)
        {
            if (!_selectedDeviceIds.Any()) return null;

            var sched = original ?? new DeviceSchedule();
            sched.Name       = ScheduleNameEntry.Text.Trim();
            sched.DeviceIds  = _selectedDeviceIds.ToList();
            sched.OnTime     = OnTimePicker.Time;
            sched.OffTime    = OffTimePicker.Time;

            // Desired temp
            if (int.TryParse(DesiredTempEntry.Text, out var dt))
                sched.DesiredTemp = Math.Clamp(dt, 60, 90);
            else
                sched.DesiredTemp = null;

            sched.Sunday    = SundayCheck.IsChecked;
            sched.Monday    = MondayCheck.IsChecked;
            sched.Tuesday   = TuesdayCheck.IsChecked;
            sched.Wednesday= WednesdayCheck.IsChecked;
            sched.Thursday = ThursdayCheck.IsChecked;
            sched.Friday   = FridayCheck.IsChecked;
            sched.Saturday = SaturdayCheck.IsChecked;

            sched.IsEnabled = true;
            return sched;
        }

        void ExitEditMode()
        {
            _editingScheduleId = null;
            FormTitle.Text = "Create On/Off Schedule";
            SaveScheduleButton.IsVisible   = true;
            UpdateScheduleButton.IsVisible = false;
            CancelEditButton.IsVisible     = false;
            ResetForm();
        }

        void ResetForm()
        {
            ScheduleNameEntry.Text = "";
            DesiredTempEntry.Text  = "";
            _selectedDeviceIds.Clear();
            foreach (var child in DeviceSelectionLayout.Children.OfType<HorizontalStackLayout>())
                if (child.Children[0] is CheckBox cb) cb.IsChecked = false;

            OnTimePicker.Time  = TimeSpan.FromHours(8);
            OffTimePicker.Time = TimeSpan.FromHours(20);
            SundayCheck.IsChecked = MondayCheck.IsChecked =
            TuesdayCheck.IsChecked = WednesdayCheck.IsChecked =
            ThursdayCheck.IsChecked = FridayCheck.IsChecked =
            SaturdayCheck.IsChecked = false;
        }

        private async void OnScheduleEnabledToggled(object sender, ToggledEventArgs e)
        {
            if ((sender as Switch)?.BindingContext is DeviceSchedule sched)
                await DatabaseService.SaveSchedule(sched);
        }

        private async void OnRemoveScheduleClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            var sched = Vm.Schedules.FirstOrDefault(s => s.Id == id);
            if (sched == null) return;
            await DatabaseService.DeleteSchedule(sched);
            Vm.Schedules.Remove(sched);
        }

        private void OnReorderCompleted(object sender, EventArgs e)
        {
            // schedules will apply in VM
        }
    }
}
