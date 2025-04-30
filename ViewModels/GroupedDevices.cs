// Namespace imports for collections and model references
using System.Collections.Generic;                       // Provides IEnumerable<T>
using System.Collections.ObjectModel;                  // Provides ObservableCollection<T>
using SmartHomeApp.Models;                              // Provides DeviceGroup and Device models
using DeviceModel = SmartHomeApp.Models.Device;        // Alias to distinguish Device model

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// Observable collection that represents devices grouped under a specific room or category.
    /// Inherits change notification behavior for UI binding.
    /// </summary>
    public class GroupedDevices : ObservableCollection<DeviceModel>
    {
        /// <summary>
        /// Identifier of the device group (room) represented by this collection.
        /// </summary>
        public int GroupId { get; }

        /// <summary>
        /// Display name of the device group (room) represented by this collection.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Initializes a new instance using group metadata and its associated devices.
        /// </summary>
        /// <param name="group">DeviceGroup instance containing Id and GroupName.</param>
        /// <param name="devices">Sequence of DeviceModel instances belonging to the group.</param>
        public GroupedDevices(DeviceGroup group, IEnumerable<DeviceModel> devices)
            : base(devices)
        {
            GroupId = group.Id;                // Assign group identifier
            GroupName = group.GroupName;       // Assign group display name
        }
    }
}
