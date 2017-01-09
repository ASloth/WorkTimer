using System; 
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;

namespace WorkTimer.Model
{
    public class WorkWeek
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        #region Relations

        [OneToMany(CascadeOperations = CascadeOperation.CascadeInsert)]
        public List<WorkDay> WorkDays { get; set; } = new List<WorkDay>();

        #endregion 

        public DateTimeOffset FirstDateOfWeek { get; set; }

        public WorkWeek() { }

        public WorkWeek(DateTimeOffset firstDateOfWeek)
        {
            FirstDateOfWeek = firstDateOfWeek.Date;
        }

        #region formatted informations

        [Ignore]
        public string WeekYearInfo
        {
            get
            {
                DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dfi.Calendar;
                var calenderWeek = cal.GetWeekOfYear(FirstDateOfWeek.DateTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

                return $"Woche {calenderWeek}, {FirstDateOfWeek.Year}";
            }
        }

        [Ignore]
        public string WorkedHourInfo => $"{TotalWorkedTime.ToString("hh\\:mm\\:ss")} gearbeitet, {TotalBreakTime.ToString("hh\\:mm\\:ss")} Pause insgesamt.";

        #endregion

        #region WorkDay summorizing

        [Ignore]
        public TimeSpan TotalWorkedTimeWithoutBreaks
        {
            get
            {
                TimeSpan totalTimeSpan = TimeSpan.Zero;

                if (WorkDays != null)
                {
                    foreach (WorkDay workDay in WorkDays)
                    {
                        totalTimeSpan = totalTimeSpan.Add(workDay.WorkedTimeWithoutBreaks);
                    }
                }
               
                return totalTimeSpan;
            }
        }

        [Ignore]
        public TimeSpan TotalWorkedTime
        {
            get
            {
                TimeSpan totalTimeSpan = TimeSpan.Zero;

                if (WorkDays != null)
                {
                    foreach (WorkDay workDay in WorkDays)
                    {
                        totalTimeSpan = totalTimeSpan.Add(workDay.WorkedTime);
                    }
                }

                return totalTimeSpan;
            }
        }

        [Ignore]
        public TimeSpan TotalBreakTime
        {
            get
            {
                TimeSpan totalTimeSpan = TimeSpan.Zero;

                if (WorkDays != null)
                {
                    foreach (WorkDay workDay in WorkDays)
                    {
                        totalTimeSpan = totalTimeSpan.Add(workDay.TotalBreakTime);
                    }
                }

                return totalTimeSpan; 
            }
        }

        #endregion

        #region Average times

        [Ignore]
        public TimeSpan AverageWorkedTimeWithoutBreaks
        {
            get
            {
                if (WorkDays == null || !WorkDays.Any()) return TimeSpan.Zero;

                return TimeSpan.FromTicks(TotalWorkedTimeWithoutBreaks.Ticks / WorkDays.Count);
            }
        }

        [Ignore]
        public TimeSpan AverageWorkedTime
        {
            get
            {
                if (WorkDays == null || !WorkDays.Any()) return TimeSpan.Zero;

                return TimeSpan.FromTicks(TotalWorkedTime.Ticks / WorkDays.Count);
            }
        }

        [Ignore]
        public TimeSpan AverageBreakTime
        {
            get
            {
                if (WorkDays == null || !WorkDays.Any()) return TimeSpan.Zero;

                return TimeSpan.FromTicks(TotalBreakTime.Ticks / WorkDays.Count);
            }
        }

        #endregion
    }
}