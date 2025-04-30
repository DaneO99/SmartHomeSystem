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
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<DeviceModel>    Devices   { get; } = new();
        public ObservableCollection<DeviceGroup>    Groups    { get; } = new();
        public ObservableCollection<DeviceSchedule> Schedules { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainViewModel()
        {
            LoadData();
            StartScheduler();
        }

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

            OnPropertyChanged(nameof(Devices));
            OnPropertyChanged(nameof(Groups));
            OnPropertyChanged(nameof(Schedules));
        }

        void StartScheduler()
        {
            var timer = new System.Timers.Timer(60_000);
            timer.Elapsed += async (_,_) => await ApplySchedules();
            timer.Start();
        }

        async Task ApplySchedules()
        {
            var now = DateTime.Now.TimeOfDay;
            var dirty = false;

            foreach (var sched in Schedules.Where(s => s.IsEnabled))
            {
                foreach (var id in sched.DeviceIds)
                {
                    var dev = Devices.FirstOrDefault(d => d.Id == id);
                    if (dev == null) continue;

                    // Thermostat vs On/Off
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

                        // store/clear originalTemp
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

            if (dirty)
                OnPropertyChanged(nameof(Devices));
        }

        public async Task ToggleAllOfType(string type, bool turnOn)
        {
            foreach (var d in Devices.Where(d => d.Type.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                d.IsOn = turnOn;
                await DatabaseService.SaveDevice(d);
            }
            OnPropertyChanged(nameof(Devices));
        }

        /// <summary>
        /// Creates a new schedule.  Pass desiredTemp only for thermostats.
        /// </summary>
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
        /// Deletes an existing schedule.
        /// </summary>
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
