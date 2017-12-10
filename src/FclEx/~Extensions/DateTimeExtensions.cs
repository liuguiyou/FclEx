using System;

namespace FclEx
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Unix时间戳
        /// 自1970年1月1日0时起的秒数
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long ToTimestamp(this DateTime d)
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalSeconds;
        }

        public static long ToTimestampMilli(this DateTime d)
        {
            return (long)(DateTime.UtcNow - Jan1St1970).TotalMilliseconds;
        }
    }
}
