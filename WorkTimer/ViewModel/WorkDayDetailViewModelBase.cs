using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MvvmNano;
using WorkTimer.ExtensionMethods;
using WorkTimer.Interface;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class WorkDayDetailViewModelBase<TViewModel> : MvvmNanoViewModel<TViewModel> where TViewModel : WorkDay
    {
        private const string NO_DATA = "-";

        protected IWorkManager _workManager;

        private WorkDay _todaysWorkDay;

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

        private bool _isInBreak;

        public bool IsTodaysWorkDone { get; set; }

        //Formatted infos 
        public string StartTimeFormatted => GetStartTimeFormatted();

        public string EndTimeFormatted => GetEndTimeFormatted();

        public string TodayWorkedFormatted => GetTodayWorkedFormatted();

        public string WorkLeftFormatted => GetWorkLeftFormatted();

        public string TotalBreakTimeFormatted => GetTotalBreakTimeFormatted();

        #region time infos

        public bool IsBreakPossible => GetIsBreakPossible();

        public bool CanStartWork => GetCanStartWork(); 

        public WorkDayDetailViewModelBase(IWorkManager workManager)
        {
            _workManager = workManager;
            _workManager.DayUpdatedEvent += DayUpdated;

            Device.StartTimer(TimeSpan.FromMilliseconds(250), TimerTick); 
        } 

        protected void SetDay(WorkDay workDay)
        {
            _todaysWorkDay = workDay;
        } 

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

        private void DayUpdated(object sender, WorkDay workDay)
        {
            if (!workDay.Date.IsSameDay(DateTimeOffset.Now)) return;

            Debug.WriteLine("Todays WorkDay updated.");
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
            set { BreakStateChanged(value); }
        }

        protected virtual void BreakStateChanged(bool inBreak)
        {
            
        }
        
        private bool GetCanStartWork()
        {
            return TodaysWorkDay == null || TodaysWorkDay.Start.IsNull();
        }

        private bool GetIsBreakPossible()
        {
            return TodaysWorkDay != null && !TodaysWorkDay.Start.IsNull() && TodaysWorkDay.End.IsNull();
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
    }
}