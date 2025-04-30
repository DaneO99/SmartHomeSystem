// SQLite ORM mapping and change notification support for scheduled device events
using SQLite;                                     // Provides attributes for SQLite persistence
using System;                                     // Provides basic system types (TimeSpan)
using System.Collections.Generic;                 // Provides generic List<T>
using System.ComponentModel;                      // Defines INotifyPropertyChanged interface
using System.Linq;                                // Provides LINQ extensions for collections
using System.Runtime.CompilerServices;            // Provides CallerMemberName attribute

namespace SmartHomeApp.Models
{
    /// <summary>
    /// Represents a schedule defining on/off times and repeat days for one or more devices.
    /// Supports persistence via SQLite and notifies UI of property updates.
    /// </summary>
    [Table("DeviceSchedule")]                      // Maps class to "DeviceSchedule" table
    public class DeviceSchedule : INotifyPropertyChanged
    {
        /// <summary>
        /// Auto-generated primary key for each schedule entry.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// JSON-formatted string containing comma-separated device IDs.
        /// Stored in database; converted to List<int> via <see cref="DeviceIds"/>.
        /// </summary>
        public string DeviceIdsJson { get; set; } = string.Empty;

        /// <summary>
        /// List of device identifiers to which this schedule applies.
        /// Populated by parsing <see cref="DeviceIdsJson"/> and re-serialized on set.
        /// </summary>
        [Ignore]                                     // Excluded from database persistence
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

        /// <summary>
        /// Time of day when associated devices should be turned on.
        /// </summary>
        public TimeSpan OnTime { get; set; }

        /// <summary>
        /// Time of day when associated devices should be turned off.
        /// </summary>
        public TimeSpan OffTime { get; set; }

        // Day-of-week flags indicating which days the schedule repeats
        public bool Sunday    { get; set; }
        public bool Monday    { get; set; }
        public bool Tuesday   { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday  { get; set; }
        public bool Friday    { get; set; }
        public bool Saturday  { get; set; }

        /// <summary>
        /// Indicates whether this schedule is active and should be executed.
        /// </summary>
        public bool IsEnabled { get; set; }

        // Optional thermostat-specific temperature settings

        /// <summary>
        /// Desired temperature for thermostat devices when schedule activates.
        /// Null for non-thermostat device types.
        /// </summary>
        public int? DesiredTemp { get; set; }

        /// <summary>
        /// Original temperature value before applying the schedule.
        /// Allows restoration when schedule deactivates.
        /// </summary>
        public int? OriginalTemp { get; set; }

        /// <summary>
        /// Event raised when any property value changes, enabling UI updates.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Invokes <see cref="PropertyChanged"/> for the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
