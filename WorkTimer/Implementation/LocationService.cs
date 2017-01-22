using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmNano;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions.Abstractions;
using Plugin.Vibrate.Abstractions;
using WorkTimer.Interface;

namespace WorkTimer.Implementation
{
    public class LocationService : ILocationService
    {
        private readonly IPermissions _permissions;
        private readonly IGeolocator _geolocator;
        private readonly IWorkManager _workManager;
        private readonly ISettingStorage _settingStorage;
        private readonly IVibrate _vibrate;

        private DateTimeOffset _lastReminder = DateTime.Today;

        public LocationService(IPermissions permissions, IGeolocator geolocator, IWorkManager workManager, ISettingStorage settingStorage, IVibrate vibrate)
        {
            _permissions = permissions;
            _geolocator = geolocator;
            _workManager = workManager;
            _settingStorage = settingStorage;
            _vibrate = vibrate;
        }

        public async Task StartService()
        {
            if (await RequestPermission())
            {
                _geolocator.PositionChanged += GeolocationOnPositionChanged;

                await _geolocator.StartListeningAsync(1000, 10);
            }
        }

        async Task<bool> RequestPermission()
        {
            if (await _permissions.CheckPermissionStatusAsync(Permission.Location) != PermissionStatus.Granted)
            {
                await _permissions.RequestPermissionsAsync(Permission.Location); //TODO Check result
            }

            return await _permissions.CheckPermissionStatusAsync(Permission.Location) == PermissionStatus.Granted;
        }

        private async void GeolocationOnPositionChanged(object sender, PositionEventArgs positionEventArgs)
        { 
            var position = positionEventArgs.Position; 

            if (_settingStorage.Location.DistanceTo(position.Latitude, position.Longitude) < _settingStorage.Distance)
            {
                //User is inside the selected range -> User is at his workplace!
                await UserInside();
            }
            else
            {
                await UserOutside();
            }
        }

        bool ShouldAlertUser()
        {
            return DateTimeOffset.Now - _lastReminder > _settingStorage.AlertSnoozeTime;
        }

        private async Task UserOutside()
        {
            var timerRunning = await _workManager.DayStarted();

            var currentDay = await _workManager.GetDay(DateTimeOffset.Now); 

            if (timerRunning && !currentDay.WorkEnded && ShouldAlertUser())
            {
                await AlertUserEndWork();
            }
        }

        private async Task UserInside()
        {
            var timerRunning = await _workManager.DayStarted(); 

            if (ShouldAlertUser() && !timerRunning)
            { 
                await AlertUserStartWork();
            }
        }

        private async Task AlertUserStartWork()
        {
            Debug.WriteLine("INFO: Inform user to start work");

            _lastReminder = DateTimeOffset.Now;
            _vibrate.Vibration();
            //TODO add graphical information 
        }

        private async Task AlertUserEndWork()
        {
            Debug.WriteLine("INFO: Inform user to end work");

            _lastReminder = DateTimeOffset.Now;
            _vibrate.Vibration();
            //TODO add graphical information
        } 
    }
}