using System;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;

namespace WorkTimer.Model
{
    public class Break
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        #region relations

        [ForeignKey(typeof(WorkDay))]
        public int WorkDayId { get; set; } 

        #endregion

        #region extensions

        [Ignore]
        public bool IsFinished => !End.IsNull();

        [Ignore]
        public TimeSpan Total => GetTotal();

        private TimeSpan GetTotal()
        {
            if (!IsFinished) return DateTimeOffset.Now - Start;

            return End - Start;
        }

        #endregion
    }
}