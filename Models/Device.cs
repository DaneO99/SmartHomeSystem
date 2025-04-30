// SQLite ORM mapping and change notification support
using SQLite;                                   // Provides attributes for SQLite table mapping
using System.ComponentModel;                    // Defines INotifyPropertyChanged interface
using System.Runtime.CompilerServices;           // Provides CallerMemberName attribute

namespace SmartHomeApp.Models
{
    /// <summary>
    /// Represents a smart home device with persistence and property-change notification.
    /// </summary>
    [Table("Device")]                               // Maps class to "Device" table in SQLite
    public class Device : INotifyPropertyChanged
    {
        /// <summary>
        /// Unique identifier, auto-generated primary key.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        // Backing field for device name
        string _name = string.Empty;
        /// <summary>
        /// Gets or sets the display name of the device.
        /// Notifies when value changes.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name == value) return;  // No action if value remains unchanged
                _name = value;
                OnPropertyChanged();          // Signal UI to update bound properties
            }
        }

        // Backing field for device type
        string _type = string.Empty;
        /// <summary>
        /// Gets or sets the type classification of the device (e.g., "Light", "Thermostat").
        /// Notifies when value changes.
        /// </summary>
        public string Type
        {
            get => _type;
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        // Backing field for on/off state
        bool _isOn;
        /// <summary>
        /// Gets or sets the power state of the device (true = on, false = off).
        /// Notifies when value changes.
        /// </summary>
        public bool IsOn
        {
            get => _isOn;
            set
            {
                if (_isOn == value) return;
                _isOn = value;
                OnPropertyChanged();
            }
        }

        // Backing field for thermostat temperature
        int _temperature;
        /// <summary>
        /// Gets or sets the current temperature setting (valid for thermostats).
        /// Notifies when value changes.
        /// </summary>
        public int Temperature
        {
            get => _temperature;
            set
            {
                if (_temperature == value) return;
                _temperature = value;
                OnPropertyChanged();
            }
        }

        // Backing field for room or group association
        int? _groupId;
        /// <summary>
        /// Gets or sets the identifier of the group (room) containing the device.
        /// Null when no group association exists.
        /// Notifies when value changes.
        /// </summary>
        public int? GroupId
        {
            get => _groupId;
            set
            {
                if (_groupId == value) return;
                _groupId = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Event triggered when a property value changes to update data bindings.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event for the specified property name.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        void OnPropertyChanged([CallerMemberName] string name = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
