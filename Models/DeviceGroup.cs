// SQLite ORM mapping and collection support for device grouping
using SQLite;                                           // Provides attributes for SQLite table mapping
using System.Collections.ObjectModel;                   // Provides ObservableCollection<T> for dynamic data binding

namespace SmartHomeApp.Models
{
    /// <summary>
    /// Represents a named group that contains multiple devices for organizational purposes.
    /// </summary>
    public class DeviceGroup
    {
        /// <summary>
        /// Unique identifier, auto-generated primary key for the device group.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Name assigned to the group (e.g., a room or category name).
        /// </summary>
        public string GroupName { get; set; } = string.Empty;

        /// <summary>
        /// Collection of devices associated with this group.
        /// Ignored for database persistence; managed in application logic.
        /// </summary>
        [Ignore]
        public ObservableCollection<Device> DevicesInGroup { get; set; } =
            new ObservableCollection<Device>();
    }
}
