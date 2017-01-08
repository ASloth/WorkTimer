using System; 
using System.Collections.Generic;
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

        [OneToMany]
        public List<WorkDay> WorkDays { get; set; } = new List<WorkDay>();

        #endregion 

        public DateTimeOffset FirstDateOfWeek { get; set; }

        public WorkWeek() { }

        public WorkWeek(DateTimeOffset firstDateOfWeek)
        {
            FirstDateOfWeek = firstDateOfWeek.Date;
        }

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