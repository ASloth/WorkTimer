using System; 
using System.Collections.Generic;
using System.Linq;
using Realms;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;

namespace WorkTimer.Model 
{
    public class WorkDay
    {
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        public IList<Break> Breaks { get; set; }

        #region extensions

        public bool WorkStarted => !Start.IsNull();

        public bool WorkEnded => !End.IsNull();

        public TimeSpan WorkedTimeWithoutBreaks => GetWorkedTimeWithoutBreaks();

        private TimeSpan GetWorkedTimeWithoutBreaks()
        {
            if (Start.IsNull()) return TimeSpan.Zero;

            if (End.IsNull())
                return DateTimeOffset.Now - Start;
            else
                return End - Start;
        }

        public TimeSpan WorkedTime => GetWorkedTime();

        private TimeSpan GetWorkedTime()
        {
            return GetWorkedTimeWithoutBreaks() - GetTotalBreakTime();
        }

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
            Start = DateTimeOffset.Now.Subtract(TimeSpan.FromHours(2));
        }

        public void EndWork()
        {
            End = DateTimeOffset.Now;
        }

        #endregion

        #region break methods

        public void StartBreak()
        {
            if(IsInBreak()) throw new AlreadyInBreakException();

            if (Breaks == null) Breaks = new List<Break>();

            Breaks.Add(new Break()
            {
                Start = DateTimeOffset.Now
            }); 
        }

        public void EndBreak()
        {
            if (!IsInBreak()) throw new NotInBreakException();

            LastBreak.End = DateTimeOffset.Now;
        }

        #endregion
    }
}