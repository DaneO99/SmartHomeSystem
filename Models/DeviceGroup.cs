using SQLite;
using System.Collections.ObjectModel;

namespace SmartHomeApp.Models
{
    public class DeviceGroup
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string GroupName { get; set; } = string.Empty;

        [Ignore]
        public ObservableCollection<Device> DevicesInGroup { get; set; } = new ObservableCollection<Device>();
    }
}
