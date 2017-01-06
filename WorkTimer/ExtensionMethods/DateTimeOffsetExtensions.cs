using System;

namespace WorkTimer.ExtensionMethods
{
    public static class DateTimeOffsetExtensions
    {
        public static bool IsNull(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset == default(DateTimeOffset);
        }
    }
}