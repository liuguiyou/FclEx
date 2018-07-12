using System;

namespace FclEx
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime _jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Unix时间戳
        /// 自1970年1月1日0时起的秒数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime d)
        {
            return (long)(DateTime.UtcNow - _jan1St1970).TotalSeconds;
        }

        public static long ToTimestampMilli(this DateTime d)
        {
            return (long)(DateTime.UtcNow - _jan1St1970).TotalMilliseconds;
        }

        public static string ToShort(this DateTime @this) => @this.ToString("yyyyMMddHHmmss");

        public static string ToCn(this DateTime @this) => @this.ToString("yyyy-MM-dd HH:mm:ss");

        public static DateTime AddWeeks(this DateTime dateTime, int numberOfWeeks)
        {
            return dateTime.AddDays(numberOfWeeks * 7);
        }
    }
}
