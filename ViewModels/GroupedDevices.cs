using System.Collections.Generic;
using System.Collections.ObjectModel;
using SmartHomeApp.Models;
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.ViewModels
{
    /// <summary>
    /// A helper collection that groups DeviceModel instances under a single room (DeviceGroup).
    /// </summary>
    public class GroupedDevices : ObservableCollection<DeviceModel>
    {
        public int GroupId { get; }
        public string GroupName { get; }

        public GroupedDevices(DeviceGroup group, IEnumerable<DeviceModel> devices)
            : base(devices)
        {
            GroupId = group.Id;
            GroupName = group.GroupName;
        }
    }
}
