// Imports for data binding, collections, LINQ, and async operations
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using DeviceModel    = SmartHomeApp.Models.Device;
using DeviceGroup    = SmartHomeApp.Models.DeviceGroup;
using DeviceSchedule = SmartHomeApp.Models.DeviceSchedule;
using SmartHomeApp.Services;

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// Central view model providing collections and operations for devices,
    /// groups, and schedules within the application.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Collection of all device models for UI binding.
        /// </summary>
        public ObservableCollection<DeviceModel> Devices { get; } = new();

        /// <summary>
        /// Collection of all device groups (rooms) for UI binding.
        /// </summary>
        public ObservableCollection<DeviceGroup> Groups { get; } = new();

        /// <summary>
        /// Collection of all device schedules for UI binding.
        /// </summary>
        public ObservableCollection<DeviceSchedule> Schedules { get; } = new();

        /// <summary>
        /// Event raised when a property value changes, enabling UI updates.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Initializes data loading and schedule processing on creation.
        /// </summary>
        public MainViewModel()
        {
            LoadData();       // Populate collections from database
            StartScheduler(); // Begin periodic schedule application
        }

        /// <summary>
        /// Asynchronously loads device, group, and schedule data from the database,
        /// clearing existing collections before adding fresh entries.
        /// </summary>
        async void LoadData()
        {
            await DatabaseService.InitializeAsync();

            Devices.Clear();
            foreach (var d in await DatabaseService.GetDevices())
                Devices.Add(d);

            Groups.Clear();
            foreach (var g in await DatabaseService.GetDeviceGroups())
                Groups.Add(g);

            Schedules.Clear();
            foreach (var s in await DatabaseService.GetSchedules())
                Schedules.Add(s);

            // Notify UI of collection updates
            OnPropertyChanged(nameof(Devices));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Schedules));
        }

        /// <summary>
        /// Configures a timer to execute ApplySchedules every minute.
        /// </summary>
        void StartScheduler()
        {
            var timer = new System.Timers.Timer(60_000);  // Interval: 60 seconds
            timer.Elapsed += async (_, _) => await ApplySchedules();
            timer.Start();                                // Begin timer
        }

        /// <summary>
        /// Applies active schedules to matching devices based on current time,
        /// updating states and thermostat settings as configured.
        /// </summary>
        async Task ApplySchedules()
        {
            var now = DateTime.Now.TimeOfDay;
            var dirty = false;  // Tracks whether device collection requires UI refresh

            // Process each enabled schedule
            foreach (var sched in Schedules.Where(s => s.IsEnabled))
            {
                foreach (var id in sched.DeviceIds)
                {
                    var dev = Devices.FirstOrDefault(d => d.Id == id);
                    if (dev == null)
                        continue;  // Skip missing devices

                    // Handle thermostat schedules when DesiredTemp is set
                    if (sched.DesiredTemp.HasValue)
                    {
                        bool inWindow = now >= sched.OnTime && now < sched.OffTime;
                        var target = inWindow
                           ? sched.DesiredTemp.Value
                           : (sched.OriginalTemp ?? dev.Temperature);

                        if (dev.Temperature != target)
                        {
                            dev.Temperature = target;
                            await DatabaseService.SaveDevice(dev);
                            dirty = true;
                        }

                        // Manage OriginalTemp storage based on schedule window
                        if (inWindow && !sched.OriginalTemp.HasValue)
                        {
                            sched.OriginalTemp = dev.Temperature;
                            await DatabaseService.SaveSchedule(sched);
                        }
                        else if (!inWindow && sched.OriginalTemp.HasValue)
                        {
                            sched.OriginalTemp = null;
                            await DatabaseService.SaveSchedule(sched);
                        }
                    }
                    else
                    {
                        // Handle simple on/off schedules
                        bool shouldOn = now >= sched.OnTime && now < sched.OffTime;
                        if (dev.IsOn != shouldOn)
                        {
                            dev.IsOn = shouldOn;
                            await DatabaseService.SaveDevice(dev);
                            dirty = true;
                        }
                    }
                }
            }

            // Refresh device collection in UI if any changes occurred
            if (dirty)
                OnPropertyChanged(nameof(Devices));
        }

        /// <summary>
        /// Toggles power state for all devices matching the specified type.
        /// </summary>
        /// <param name="type">Device type to filter (case-insensitive).</param>
        /// <param name="turnOn">Desired on/off state.</param>
        public async Task ToggleAllOfType(string type, bool turnOn)
        {
            foreach (var d in Devices.Where(d => d.Type.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                d.IsOn = turnOn;
                await DatabaseService.SaveDevice(d);
            }
            OnPropertyChanged(nameof(Devices));  // Notify UI of bulk update
        }

        /// <summary>
        /// Creates and persists a new schedule for a single device;
        /// includes DesiredTemp only for thermostat devices.
        /// </summary>
        /// <param name="device">Device to schedule.</param>
        /// <param name="on">Activation time of day.</param>
        /// <param name="off">Deactivation time of day.</param>
        /// <param name="desiredTemp">Optional thermostat temperature setting.</param>
        public async Task AddSchedule(
            DeviceModel device,
            TimeSpan on,
            TimeSpan off,
            int? desiredTemp = null)
        {
            var sched = new DeviceSchedule
            {
                DeviceIds   = new List<int> { device.Id },
                OnTime      = on,
                OffTime     = off,
                DesiredTemp = desiredTemp,
                IsEnabled   = true
            };
            await DatabaseService.SaveSchedule(sched);
            Schedules.Add(sched);
            OnPropertyChanged(nameof(Schedules));
        }

        /// <summary>
        /// Removes a schedule by its identifier and updates UI.
        /// </summary>
        /// <param name="scheduleId">Identifier of the schedule to delete.</param>
        public async Task RemoveSchedule(int scheduleId)
        {
            var sched = Schedules.FirstOrDefault(s => s.Id == scheduleId);
            if (sched != null)
            {
                await DatabaseService.DeleteSchedule(sched);
                Schedules.Remove(sched);
                OnPropertyChanged(nameof(Schedules));
            }
        }

        /// <summary>
        /// Invokes PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
