using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Dispatching;    // For MainThread
using SmartHomeApp.Services;         // For DatabaseService

// Model aliases to avoid conflicts
using DeviceModel   = SmartHomeApp.Models.Device;
using GroupModel    = SmartHomeApp.Models.DeviceGroup;
using ScheduleModel = SmartHomeApp.Models.DeviceSchedule;

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// Central ViewModel:
    /// - Loads Devices, Groups, and Schedules from SQLite
    /// - Exposes:
    ///   * Devices (master list)
    ///   * Groups  (rooms)
    ///   * Lights, DoorLocks, Thermostats (filtered lists)
    /// - Runs a UI-thread timer to apply schedules
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // --- Collections for data binding ---
        public ObservableCollection<DeviceModel> Devices      { get; } = new();
        public ObservableCollection<GroupModel>  Groups       { get; } = new();
        public ObservableCollection<ScheduleModel> Schedules   { get; } = new();

        // Filtered sub-lists for DevicePage accordions
        public ObservableCollection<DeviceModel> Lights       { get; } = new();
        public ObservableCollection<DeviceModel> DoorLocks    { get; } = new();
        public ObservableCollection<DeviceModel> Thermostats  { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        bool _timerRunning;

        public MainViewModel()
        {
            // Keep filtered lists in sync with Devices
            Devices.CollectionChanged += OnDevicesCollectionChanged;
            _ = InitializeAsync();
        }

        /// <summary>
        /// Initialize DB, load devices/groups/schedules, partition devices, start scheduler.
        /// </summary>
        public async Task InitializeAsync()
        {
            await DatabaseService.InitializeAsync();

            // Load Rooms
            var roomList = await DatabaseService.GetDeviceGroups();
            foreach (var g in roomList)
                Groups.Add(g);

            // Load Devices
            var deviceList = await DatabaseService.GetDevices();
            foreach (var d in deviceList)
                Devices.Add(d);

            // Load Schedules
            var schedList = await DatabaseService.GetSchedules();
            foreach (var s in schedList)
                Schedules.Add(s);

            // Build filtered lists
            PartitionDevices();

            // Notify UI of initial load
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(Groups));
                OnPropertyChanged(nameof(Devices));
                OnPropertyChanged(nameof(Schedules));
            });

            StartScheduler();
        }

        // Rebuild filtered lists when Devices changes
        void OnDevicesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
            => PartitionDevices();

        void PartitionDevices()
        {
            Lights.Clear();
            DoorLocks.Clear();
            Thermostats.Clear();

            foreach (var d in Devices)
            {
                switch (d.Type?.Trim().ToLowerInvariant())
                {
                    case "light":
                        Lights.Add(d);
                        break;
                    case "door lock":
                        DoorLocks.Add(d);
                        break;
                    case "thermostat":
                        Thermostats.Add(d);
                        break;
                }
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnPropertyChanged(nameof(Lights));
                OnPropertyChanged(nameof(DoorLocks));
                OnPropertyChanged(nameof(Thermostats));
            });
        }

        void StartScheduler()
        {
            if (_timerRunning) return;
            _timerRunning = true;

            Application.Current?.Dispatcher.StartTimer(
                TimeSpan.FromMinutes(1),
                () =>
                {
                    _ = ApplySchedulesAsync();
                    return _timerRunning;
                });
        }

        public void StopScheduler() => _timerRunning = false;

        async Task ApplySchedulesAsync()
        {
            var now = DateTime.Now.TimeOfDay;
            var dirty = false;

            foreach (var sched in Schedules.Where(s => s.IsEnabled))
            {
                foreach (var id in sched.DeviceIds)
                {
                    var d = Devices.FirstOrDefault(x => x.Id == id);
                    if (d == null) continue;

                    if (sched.DesiredTemp.HasValue)
                    {
                        bool inWindow = now >= sched.OnTime && now < sched.OffTime;
                        int target = inWindow
                            ? sched.DesiredTemp.Value
                            : (sched.OriginalTemp ?? d.Temperature);

                        if (d.Temperature != target)
                        {
                            d.Temperature = target;
                            await DatabaseService.SaveDevice(d);
                            dirty = true;
                        }

                        if (inWindow && !sched.OriginalTemp.HasValue)
                        {
                            sched.OriginalTemp = d.Temperature;
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
                        bool shouldOn = now >= sched.OnTime && now < sched.OffTime;
                        if (d.IsOn != shouldOn)
                        {
                            d.IsOn = shouldOn;
                            await DatabaseService.SaveDevice(d);
                            dirty = true;
                        }
                    }
                }
            }

            if (dirty)
                MainThread.BeginInvokeOnMainThread(() => OnPropertyChanged(nameof(Devices)));
        }

        public async Task ToggleAllOfType(string type, bool turnOn)
        {
            foreach (var d in Devices.Where(x =>
                x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                d.IsOn = turnOn;
                await DatabaseService.SaveDevice(d);
            }
            OnPropertyChanged(nameof(Devices));
        }

        public async Task AddSchedule(
            DeviceModel device,
            TimeSpan on,
            TimeSpan off,
            int? desiredTemp = null)
        {
            var sched = new ScheduleModel
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

        void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
