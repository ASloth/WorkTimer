using System;
using MvvmNano;
using WorkTimer.ExtensionMethods;
using WorkTimer.Model;
using Xamarin.Forms;

namespace WorkTimer.ViewModel
{
    public class MainViewModel : MvvmNanoViewModel
    {
        #region dic

        private const string NO_DATA = "-";

        #endregion

        private bool _isInBreak;

        public WorkDay TodaysWorkDay { get; set; }

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
        public MvvmNanoCommand StartWorkCommand => new MvvmNanoCommand(StartWork);

        public MvvmNanoCommand EndWorkCommand => new MvvmNanoCommand(EndWork);

        public MainViewModel()
        {
            Device.StartTimer(TimeSpan.FromMilliseconds(250), TimerTick);
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

        private void StartWork()
        {
            TodaysWorkDay = new WorkDay();
            TodaysWorkDay.StartWork();
            NotifyPropertyChanged(nameof(StartTimeFormatted));
            NotifyPropertyChanged(nameof(CanStartWork));
            NotifyPropertyChanged(nameof(CanEndWork));
            NotifyPropertyChanged(nameof(IsBreakPossible));
        }

        private void EndWork()
        {
            TodaysWorkDay.EndWork();
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
                    StartBreak();
                }
                if (!value && TodaysWorkDay.IsInBreak()) //Was in break, end break.
                {
                    EndBreak();
                } 
            }
        }

        private void StartBreak()
        {
            TodaysWorkDay.StartBreak();
            NotifyPropertyChanged(nameof(IsInBreak));
        }

        private void EndBreak()
        {
            TodaysWorkDay.EndBreak();
            NotifyPropertyChanged(nameof(IsInBreak));
        }
    }
}