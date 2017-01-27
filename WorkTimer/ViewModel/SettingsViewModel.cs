using System;
using MvvmNano;
using WorkTimer.Interface;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class SettingsViewModel : MvvmNanoViewModel
    {
        private readonly ISettingStorage _settingStorage; 

        public bool CheckLocation
        {
            get { return _settingStorage.CheckLocation; }
            set
            { 
                SetCheckLocation(value);
            }
        }
        
        public double Latitude
        {
            get { return _settingStorage.Location.Latitude; }
            set
            {  
                SetLatitude(value);
            }
        } 

        public double Longitude
        {
            get { return _settingStorage.Location.Longitude; }
            set
            { 
                SetLongitude(value);
            }
        }
        
        public bool CanSnooze
        {
            get { return _settingStorage.SnoozeAlerts; }
            set { SetCanSnooze(value); }
        }

        private void SetCanSnooze(bool value)
        {
            _settingStorage.SnoozeAlerts = value;
        }

        public double SnoozeInterval
        {
            get { return _settingStorage.AlertSnoozeTime.TotalMinutes; }
            set
            {
                SetSnoozeInterval(value);  
            }
        }
        
        public string SnoozeIntervalRoundFormatted
        {
            get
            {
                var round = (int) SnoozeInterval;
                return round < 10 ? "0" + round : round.ToString();
            }
        }

        public SettingsViewModel(ISettingStorage settingStorage)
        {
            _settingStorage = settingStorage;
        }

        private void SetCheckLocation(bool checkLocation)
        {
            _settingStorage.CheckLocation = checkLocation;
        }

        private void SetLatitude(double latitude)
        {
            _settingStorage.Location.Latitude = latitude;
        }

        private void SetLongitude(double value)
        {
            _settingStorage.Location.Longitude = value;
        }

        private void SetSnoozeInterval(double value)
        {
            _settingStorage.AlertSnoozeTime = TimeSpan.FromMinutes(value);
            NotifyPropertyChanged(nameof(SnoozeIntervalRoundFormatted));
        }

    }
}