using System;
using WorkTimer.Exceptions;
using WorkTimer.ExtensionMethods;

namespace WorkTimer.Model
{
    public class Break
    {
        public DateTimeOffset Start { get; set; }

        public DateTimeOffset End { get; set; }

        #region extensions

        public bool IsFinished => !End.IsNull();

        public TimeSpan Total => GetTotal();

        private TimeSpan GetTotal()
        {
            if (!IsFinished) return DateTimeOffset.Now - Start;

            return End - Start;
        }

        #endregion
    }
}