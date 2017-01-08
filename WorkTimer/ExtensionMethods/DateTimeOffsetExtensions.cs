using System;
using System.Globalization;

namespace WorkTimer.ExtensionMethods
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsNull(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset == default(DateTimeOffset);
        }

        public static bool IsSameDay(this DateTimeOffset date, DateTimeOffset comparisonDay)
        {
            return date.Date.Equals(comparisonDay.Date);
        }

        /// <summary>
        /// Checks if two dates are in the same week. A week starts at monday and ends at sunday.
        /// </summary>  
        public static bool IsSameWeek(this DateTimeOffset date, DateTimeOffset comparisonDay)
        {
            DateTimeOffset beginningOfWeekDate1 = GetFirstDayOfWeek(date);
            DateTimeOffset beginningOfWeekDate2 = GetFirstDayOfWeek(comparisonDay);
            return beginningOfWeekDate1.Date.Equals(beginningOfWeekDate2.Date);
        }
        
        /// <summary>
        /// Returns a new data that represents the first date of the given week. Monday is considered as the first day of the week.
        /// </summary> 
        public static DateTimeOffset GetFirstDayOfWeek(this DateTimeOffset day)
        {
            switch (day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return day;
                case DayOfWeek.Tuesday:
                    return day.AddDays(-1);
                case DayOfWeek.Wednesday:
                    return day.AddDays(-2);
                case DayOfWeek.Thursday:
                    return day.AddDays(-3);
                case DayOfWeek.Friday:
                    return day.AddDays(-4);
                case DayOfWeek.Saturday:
                    return day.AddDays(-5);
                case DayOfWeek.Sunday:
                    return day.AddDays(-6);
                default:
                    throw new Exception();
            }
        }
    }
}