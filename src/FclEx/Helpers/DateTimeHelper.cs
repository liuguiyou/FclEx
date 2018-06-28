using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Helpers
{
    public static class DateTimeHelper
    {
        private static readonly DateTime _jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime TimestampToDateTime(long timestamp) => _jan1St1970.AddSeconds(timestamp);

        public static DateTime TimestampMilliToDateTime(long timestampMilli) => TimestampToDateTime((long)Math.Round(timestampMilli / 1000d));

        public static DateTime ThisMonth(int day, int hour = 0, int minute = 0)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, day, hour, minute, 0);
        }

        public static DateTime LastDayOfThisMonth()
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
        }

        public static DateTime ThisYear(int month, int day, int hour = 0, int minute = 0) 
            => new DateTime(DateTime.Now.Year, month, day, hour, minute, 0);

        public static DateTime Yesterday(int hour, int minute) => Today(hour, minute).AddDays(-1);

        public static DateTime Today(int hour, int minute)
        {
            var now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day, hour, minute, 0);
        }

        public static DateTime Tomorrow(int hour, int minute) => Today(hour, minute).AddDays(1);

    }
}
