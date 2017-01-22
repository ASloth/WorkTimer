using System;
using WorkTimer.Interface;
using WorkTimer.Model;

namespace WorkTimer.Implementation
{
    public class SettingStorage : ISettingStorage
    {
        private Location _location;

        public Location Location { get; set; } = new Location();

        public double Distance { get; set; } = 0.500;
        public bool SnoozeAlerts { get; set; } = true;
        public TimeSpan AlertSnoozeTime { get; set; } = TimeSpan.FromSeconds(10);
    }
}