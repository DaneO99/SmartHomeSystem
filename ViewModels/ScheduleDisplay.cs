using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SmartHomeApp.ViewModels
{
    public class ScheduleDisplay : INotifyPropertyChanged
    {
        public int Id { get; set; }

        private string _deviceName = "";
        public string DeviceName
        {
            get => _deviceName;
            set { if (_deviceName != value) { _deviceName = value; OnPropertyChanged(); } }
        }

        private TimeSpan _onTime;
        public TimeSpan OnTime { get => _onTime; set { if(_onTime!=value){_onTime=value;OnPropertyChanged();} } }

        private TimeSpan _offTime;
        public TimeSpan OffTime { get => _offTime; set { if(_offTime!=value){_offTime=value;OnPropertyChanged();} } }

        private bool _sunday;
        public bool Sunday { get => _sunday; set { if(_sunday!=value){_sunday=value;OnPropertyChanged();} } }
        private bool _monday;
        public bool Monday { get => _monday; set { if(_monday!=value){_monday=value;OnPropertyChanged();} } }
        private bool _tuesday;
        public bool Tuesday { get => _tuesday; set { if(_tuesday!=value){_tuesday=value;OnPropertyChanged();} } }
        private bool _wednesday;
        public bool Wednesday { get => _wednesday; set { if(_wednesday!=value){_wednesday=value;OnPropertyChanged();} } }
        private bool _thursday;
        public bool Thursday { get => _thursday; set { if(_thursday!=value){_thursday=value;OnPropertyChanged();} } }
        private bool _friday;
        public bool Friday { get => _friday; set { if(_friday!=value){_friday=value;OnPropertyChanged();} } }
        private bool _saturday;
        public bool Saturday { get => _saturday; set { if(_saturday!=value){_saturday=value;OnPropertyChanged();} } }

        private int? _desiredTemp;
        public int? DesiredTemp
        {
            get => _desiredTemp;
            set { if (_desiredTemp != value) { _desiredTemp = value; OnPropertyChanged(); } }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set { if (_isEnabled != value) { _isEnabled = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string n = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
