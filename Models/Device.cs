using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartHomeApp.Models
{
    [Table("Device")]
    public class Device : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { if (_name == value) return; _name = value; OnPropertyChanged(); }
        }

        string _type = string.Empty;
        public string Type
        {
            get => _type;
            set { if (_type == value) return; _type = value; OnPropertyChanged(); }
        }

        bool _isOn;
        public bool IsOn
        {
            get => _isOn;
            set { if (_isOn == value) return; _isOn = value; OnPropertyChanged(); }
        }

        int _temperature;
        public int Temperature
        {
            get => _temperature;
            set { if (_temperature == value) return; _temperature = value; OnPropertyChanged(); }
        }

        // ← This was missing
        int? _groupId;
        public int? GroupId
        {
            get => _groupId;
            set { if (_groupId == value) return; _groupId = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
