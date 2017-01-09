using System; 
using System.Collections.Generic;
using System.Linq;
using SQLite.Net;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;

namespace WorkTimer.Model 
{
    public class WorkDay
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        #region Relation

        [ForeignKey(typeof(WorkWeek))]
        public int WorkWeekId { get; set; }

        //[OneToMany]
        //public WorkWeek WorkWeek { get; set; } 

        [OneToMany]
        public List<Break> Breaks { get; set; } = new List<Break>();

        #endregion 

        public DateTimeOffset Date { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        

        public WorkDay()
        {
            
        }

        public WorkDay(DateTimeOffset date)
        {
            Date = date.Date;
        }

        #region formatted info

        [Ignore]
        public string DayOfWeek => Date.DayOfWeek.ToString();

#endregion

        #region extensions

        [Ignore]
        public bool WorkStarted => !Start.IsNull();

        [Ignore]
        public bool WorkEnded => !End.IsNull();

        [Ignore]
        public TimeSpan WorkedTimeWithoutBreaks => GetWorkedTimeWithoutBreaks();

        private TimeSpan GetWorkedTimeWithoutBreaks()
        {
            if (Start.IsNull()) return TimeSpan.Zero;

            if (End.IsNull())
                return DateTimeOffset.Now - Start;
            else
                return End - Start;
        }

        [Ignore]
        public TimeSpan WorkedTime => GetWorkedTime();

        private TimeSpan GetWorkedTime()
        {
            return GetWorkedTimeWithoutBreaks() - GetTotalBreakTime();
        }

        [Ignore]
        public TimeSpan TotalBreakTime => GetTotalBreakTime();

        private TimeSpan GetTotalBreakTime()
        {
            TimeSpan total = TimeSpan.Zero;

            if (Breaks == null) return total;

            foreach (Break b in Breaks)
            {
                 total = total.Add(b.Total);
            }
            return total;
        }

        [Ignore]
        public Break LastBreak => Breaks.Last();  

        public bool IsInBreak()
        {
            if (Breaks == null || !Breaks.Any()) return false;

            return !Breaks.Last().IsFinished;
        }

        #endregion

        #region work methods

        public void StartWork()
        {
            Start = DateTimeOffset.Now;
        }

        public void EndWork()
        {
            End = DateTimeOffset.Now;
        }

        #endregion 
    }
}