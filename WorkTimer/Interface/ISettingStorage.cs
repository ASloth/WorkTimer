using System;
using WorkTimer.Model;

namespace WorkTimer.Interface
{
    public interface ISettingStorage
    {
        Location Location { get; set; }

        double Distance { get; set; }

        bool SnoozeAlerts { get; set; }

        TimeSpan AlertSnoozeTime { get; set; }
    }
}