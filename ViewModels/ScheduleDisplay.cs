// Imports for time representation and property change notification
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// View model for presenting schedule entries,
    /// including device name, activation times, repeat days, and enabled state.
    /// </summary>
    public class ScheduleDisplay : INotifyPropertyChanged
    {
        /// <summary>
        /// Identifier corresponding to the underlying schedule entry.
        /// </summary>
        public int Id { get; set; }

        // Backing field for device name
        private string _deviceName = string.Empty;
        /// <summary>
        /// Display name of the associated device.
        /// Notifies UI when updated.
        /// </summary>
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if (_deviceName == value) return;
                _deviceName = value;
                OnPropertyChanged();
            }
        }

        // Backing field for activation time
        private TimeSpan _onTime;
        /// <summary>
        /// Time of day when the schedule activates.
        /// Notifies UI when updated.
        /// </summary>
        public TimeSpan OnTime
        {
            get => _onTime;
            set
            {
                if (_onTime == value) return;
                _onTime = value;
                OnPropertyChanged();
            }
        }

        // Backing field for deactivation time
        private TimeSpan _offTime;
        /// <summary>
        /// Time of day when the schedule deactivates.
        /// Notifies UI when updated.
        /// </summary>
        public TimeSpan OffTime
        {
            get => _offTime;
            set
            {
                if (_offTime == value) return;
                _offTime = value;
                OnPropertyChanged();
            }
        }

        // Backing fields and properties for repeat days
        private bool _sunday;
        /// <summary>True when schedule repeats on Sunday.</summary>
        public bool Sunday
        {
            get => _sunday;
            set
            {
                if (_sunday == value) return;
                _sunday = value;
                OnPropertyChanged();
            }
        }

        private bool _monday;
        /// <summary>True when schedule repeats on Monday.</summary>
        public bool Monday
        {
            get => _monday;
            set
            {
                if (_monday == value) return;
                _monday = value;
                OnPropertyChanged();
            }
        }

        private bool _tuesday;
        /// <summary>True when schedule repeats on Tuesday.</summary>
        public bool Tuesday
        {
            get => _tuesday;
            set
            {
                if (_tuesday == value) return;
                _tuesday = value;
                OnPropertyChanged();
            }
        }

        private bool _wednesday;
        /// <summary>True when schedule repeats on Wednesday.</summary>
        public bool Wednesday
        {
            get => _wednesday;
            set
            {
                if (_wednesday == value) return;
                _wednesday = value;
                OnPropertyChanged();
            }
        }

        private bool _thursday;
        /// <summary>True when schedule repeats on Thursday.</summary>
        public bool Thursday
        {
            get => _thursday;
            set
            {
                if (_thursday == value) return;
                _thursday = value;
                OnPropertyChanged();
            }
        }

        private bool _friday;
        /// <summary>True when schedule repeats on Friday.</summary>
        public bool Friday
        {
            get => _friday;
            set
            {
                if (_friday == value) return;
                _friday = value;
                OnPropertyChanged();
            }
        }

        private bool _saturday;
        /// <summary>True when schedule repeats on Saturday.</summary>
        public bool Saturday
        {
            get => _saturday;
            set
            {
                if (_saturday == value) return;
                _saturday = value;
                OnPropertyChanged();
            }
        }

        // Backing field for optional thermostat setting
        private int? _desiredTemp;
        /// <summary>
        /// Desired temperature for thermostat schedules;
        /// null for non-thermostat entries.
        /// Notifies UI when updated.
        /// </summary>
        public int? DesiredTemp
        {
            get => _desiredTemp;
            set
            {
                if (_desiredTemp == value) return;
                _desiredTemp = value;
                OnPropertyChanged();
            }
        }

        // Backing field for enabled state
        private bool _isEnabled;
        /// <summary>
        /// True when schedule is active and should be applied.
        /// Notifies UI when updated.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event triggered when a property changes to update data bindings.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event for a given property.
        /// </summary>
        /// <param name="propertyName">Name of the changed property.</param>
        void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
