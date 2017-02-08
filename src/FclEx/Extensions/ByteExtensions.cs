using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Extensions
{
    public static class ByteExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            var builder = new StringBuilder();
            foreach (var @byte in bytes)
            {
                builder.Append(@byte.ToString("X2"));
            }
            return builder.ToString();
        }

        public static byte[] ToBytes(this bool num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this char num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this short num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this int num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this long num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this ushort num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this uint num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this ulong num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this float num) => BitConverter.GetBytes(num);

        public static byte[] ToBytes(this double num) => BitConverter.GetBytes(num);

        private static byte[] ToBytes(IEnumerable<bool> bits, int count)
        {
            var numBytes = count / 8;
            if (count % 8 != 0) numBytes++;

            var bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            foreach (var bit in bits)
            {
                if (bit) bytes[byteIndex] |= (byte)(1 << bitIndex);
                ++bitIndex;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    ++byteIndex;
                }

            }
            return bytes;
        }

        public static byte[] ToBytes(this bool[] bits) => ToBytes(bits, bits.Length);

        public static byte[] ToBytes(this List<bool> bits) => ToBytes(bits, bits.Count);

        public static short ToInt16(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToInt16(bytes, startIndex);
        }

        public static ushort ToUInt16(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToUInt16(bytes, startIndex);
        }

        public static int ToInt32(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToInt32(bytes, startIndex);
        }

        public static uint ToUInt32(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToUInt32(bytes, startIndex);
        }

        public static long ToInt64(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToInt64(bytes, startIndex);
        }

        public static ulong ToUInt64(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToUInt64(bytes, startIndex);
        }

        public static float ToSingle(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToSingle(bytes, startIndex);
        }

        public static double ToDouble(this byte[] bytes, int startIndex = 0)
        {
            return BitConverter.ToDouble(bytes, startIndex);
        }

        public static int IndexOf(this byte[] buffer, int startIndex, params byte[] subBytes)
        {
            if (subBytes.Length > buffer.Length) return -1;

            var i = startIndex; // 主串的位置
            var j = 0; // 模式串的位置
            
            var next = GetNextArray(subBytes);

            while (i < buffer.Length && j < subBytes.Length)
            {
                if (j == -1 || buffer[i] == subBytes[j])
                {
                    // 当j为-1时，要移动的是i，当然j也要归0
                    i++;
                    j++;
                }
                else
                {
                    // i不需要回溯了
                    // i = i - j + 1;
                    j = next[j]; // j回到指定位置

                }
            }
            return j == subBytes.Length ? i - j : -1;
        }

        private static int[] GetNextArray(byte[] subBytes)
        {
            var next = new int[subBytes.Length];
            next[0] = -1;
            var j = 0;
            var k = -1;

            while (j < subBytes.Length - 1)
            {
                if (k == -1 || subBytes[j] == subBytes[k])
                {
                    if (subBytes[++j] == subBytes[++k])
                    {
                        // 当两个字符相等时要跳过
                        next[j] = next[k];
                    }
                    else
                    {
                        next[j] = k;
                    }
                }
                else
                {
                    k = next[k];
                }
            }
            return next;

        }
    }
}
