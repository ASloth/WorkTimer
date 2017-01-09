using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using MvvmNano;
using WorkTimer.ExtensionMethods;
using WorkTimer.Interface;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class MainViewModel : MvvmNanoViewModel
    {
        private readonly IWorkManager _workManager;

        #region dic

        private const string NO_DATA = "-";

        #endregion

        private bool _isInBreak;

        private WorkDay _todaysWorkDay;  

        public bool IsTodaysWorkDone { get; set; }

        public bool IsBreakPossible => GetIsBreakPossible();

        public bool CanStartWork => GetCanStartWork(); 

        public bool CanEndWork => GetCanEndWork(); 

        //Formatted infos 
        public string StartTimeFormatted => GetStartTimeFormatted();

        public string EndTimeFormatted => GetEndTimeFormatted();

        public string TodayWorkedFormatted => GetTodayWorkedFormatted();

        public string WorkLeftFormatted => GetWorkLeftFormatted();

        public string TotalBreakTimeFormatted => GetTotalBreakTimeFormatted();

        //Commands
        public MvvmNanoCommand StartWorkCommand => new MvvmNanoCommand(async ()=> await StartWork());

        public MvvmNanoCommand EndWorkCommand => new MvvmNanoCommand(async () => await EndWork());

        public MainViewModel(IWorkManager workManager)
        {
            _workManager = workManager;

            _workManager.DayUpdatedEvent += DayUpdated;

            Task.Run(async()=>
            {
                try
                {
                    await MvvmNanoIoC.Resolve<IDataService>().Initialize();
                    var workDay = await _workManager.GetDay(DateTimeOffset.Now.Date);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        TodaysWorkDay = workDay;
                    });
                }
                catch (Exception ex)
                {
                    
                }
                
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(250), TimerTick);
        }

        private void DayUpdated(object sender, WorkDay workDay)
        {
            if (!workDay.Date.IsSameDay(DateTimeOffset.Now)) return;

            Debug.WriteLine("Todays WorkDay updated.");
            
        }

        public WorkDay TodaysWorkDay
        {
            get { return _todaysWorkDay; }
            set
            {
                _todaysWorkDay = value;
                NotifyPropertyChanged(nameof(StartTimeFormatted));
                NotifyPropertyChanged(nameof(EndTimeFormatted));
                NotifyPropertyChanged(nameof(IsInBreak)); 
                NotifyPropertyChanged(nameof(IsBreakPossible));

                IsTodaysWorkDone = TodaysWorkDay.WorkEnded;
            }
        }

        #region time infos

        private bool TimerTick()
        {
            if (TodaysWorkDay != null)
            {
                NotifyPropertyChanged(nameof(TodayWorkedFormatted));
                NotifyPropertyChanged(nameof(WorkLeftFormatted));
                NotifyPropertyChanged(nameof(TotalBreakTimeFormatted));
            }
            return true;
        }

        private string GetStartTimeFormatted()
        {
            if (TodaysWorkDay == null) return NO_DATA;

            if (!TodaysWorkDay.WorkStarted) return NO_DATA;

            return TodaysWorkDay.Start.ToString("HH:mm");
        }

        private string GetEndTimeFormatted()
        {
            if (TodaysWorkDay == null) return NO_DATA;

            if (!TodaysWorkDay.WorkEnded) return NO_DATA;

            return TodaysWorkDay.End.ToString("HH:mm");
        }

        private string GetTodayWorkedFormatted()
        {
            if (TodaysWorkDay == null) return NO_DATA;

            if (!TodaysWorkDay.WorkStarted) return NO_DATA;

            return TodaysWorkDay.WorkedTime.ToString("hh\\:mm\\:ss");
        }

        private string GetWorkLeftFormatted()
        {
            if (TodaysWorkDay == null) return NO_DATA;

            if (!TodaysWorkDay.WorkStarted) return NO_DATA;

            var workDueToday = TimeSpan.FromHours(8);

            TimeSpan workLeft;

            if (workDueToday > TodaysWorkDay.WorkedTime)
            {
                workLeft = workDueToday - TodaysWorkDay.WorkedTime;
                IsTodaysWorkDone = false;
            }
            else
            {
                workLeft = TodaysWorkDay.WorkedTime - workDueToday;
                IsTodaysWorkDone = true;
            }

            var formattedTime = workLeft.ToString("hh\\:mm\\:ss");

            if (IsTodaysWorkDone) formattedTime = "- " + formattedTime;

            return formattedTime;
        }

        private string GetTotalBreakTimeFormatted()
        {
            if (TodaysWorkDay == null) return NO_DATA;

            var total = TodaysWorkDay.TotalBreakTime;

            if (total.Ticks == 0)
                return NO_DATA;
            else
                return total.ToString("hh\\:mm\\:ss");
        }

        #endregion

        #region work methods

        private async Task StartWork()
        {
            await _workManager.StartDay(TodaysWorkDay);
            NotifyPropertyChanged(nameof(StartTimeFormatted));
            NotifyPropertyChanged(nameof(CanStartWork));
            NotifyPropertyChanged(nameof(CanEndWork));
            NotifyPropertyChanged(nameof(IsBreakPossible));
        }

        private async Task EndWork()
        {
            await _workManager.EndDay(TodaysWorkDay);
            NotifyPropertyChanged(nameof(EndTimeFormatted));
            NotifyPropertyChanged(nameof(CanEndWork));
            NotifyPropertyChanged(nameof(IsBreakPossible)); 
        }

        private bool GetCanStartWork()
        {
            return TodaysWorkDay == null || TodaysWorkDay.Start.IsNull();
        }

        private bool GetCanEndWork()
        {
            return TodaysWorkDay != null && !TodaysWorkDay.Start.IsNull();
        }

        #endregion

        private bool GetIsBreakPossible()
        {
            return TodaysWorkDay != null && !TodaysWorkDay.Start.IsNull() && TodaysWorkDay.End.IsNull();
        }

        public bool IsInBreak
        {
            get
            {
                bool result = false;

                if (TodaysWorkDay != null) 
                    result = TodaysWorkDay.IsInBreak();

                return result;
            }
            set
            {
                if (value && !TodaysWorkDay.IsInBreak()) //Was not in break, start break.
                {
                    Task.Run(async () => await StartBreak());
                }
                if (!value && TodaysWorkDay.IsInBreak()) //Was in break, end break.
                {
                    Task.Run(async () => await EndBreak());
                } 
            }
        }

        private async Task StartBreak()
        {
            await _workManager.StartBreak(TodaysWorkDay);  
            NotifyPropertyChanged(nameof(IsInBreak));
        }

        private async Task EndBreak()
        {
            await _workManager.EndBreak(TodaysWorkDay);
            NotifyPropertyChanged(nameof(IsInBreak));
        }
    }
}