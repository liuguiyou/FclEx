using System;

namespace FclEx.Helpers
{
    public static class StringHelper
    {

        /// <summary>
        /// 把十六进制字符串转在byte[]
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length == 0)
            {
                return new byte[] { 0 };
            }
            if (hex.Length % 2 == 1)
            {
                hex = "0" + hex;
            }
            var result = new byte[hex.Length / 2];

            for (var i = 0; i < hex.Length / 2; i++)
            {
                result[i] = byte.Parse(hex.Substring(2 * i, 2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }
            return result;
        }

        public static string PadLeftWith(string source, int blockSize, char padChar) => PadWith(source, blockSize, padChar, true);

        public static string PadRightWith(string source, int blockSize, char padChar) => PadWith(source, blockSize, padChar, false);

        private static string PadWith(string source, int blockSize, char padChar, bool padLeft)
        {
            var padLength = blockSize - source.Length % blockSize;
            if (padLength == 0) return source;
            var totalWidth = padLength + source.Length;

            return padLeft
                ? source.PadLeft(totalWidth, padChar)
                : source.PadRight(totalWidth, padChar);
        }

        public static class DateTimeHelpers
        {
            private static readonly DateTime _jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            public static DateTime TimestampToDateTime(long timestamp) => _jan1St1970.AddSeconds(timestamp);

            public static DateTime TimestampMilliToDateTime(long timestampMilli) => TimestampToDateTime((long)Math.Round(timestampMilli / 1000d));
        }
    }
}
