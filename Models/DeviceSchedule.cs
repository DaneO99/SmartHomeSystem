using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SmartHomeApp.Models
{
    [Table("DeviceSchedule")]
    public class DeviceSchedule : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // New: user‐provided schedule name
        public string Name { get; set; } = string.Empty;

        // Comma‐separated list of device IDs
        public string DeviceIdsJson { get; set; } = string.Empty;

        [Ignore]
        public List<int> DeviceIds
        {
            get => string.IsNullOrWhiteSpace(DeviceIdsJson)
                ? new List<int>()
                : DeviceIdsJson
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            set => DeviceIdsJson = value == null
                ? string.Empty
                : string.Join(',', value);
        }

        public TimeSpan OnTime  { get; set; }
        public TimeSpan OffTime { get; set; }

        // Repeat flags
        public bool Sunday    { get; set; }
        public bool Monday    { get; set; }
        public bool Tuesday   { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday  { get; set; }
        public bool Friday    { get; set; }
        public bool Saturday  { get; set; }

        public bool IsEnabled { get; set; }

        // Thermostat settings (optional)
        public int? DesiredTemp  { get; set; }
        public int? OriginalTemp { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
