using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace FclEx.Extensions
{
    public static class Md5Extensions
    {
        private static readonly Regex RegMd5 = new Regex(@"^([a-fA-F0-9]{32})$");

        public static string ToMd5String(this byte[] input)
        {
            return input.ToMd5().ToHexString();
        }

        public static byte[] ToMd5(this byte[] input)
        {
            return MD5.Create().ComputeHash(input);
        }
        
        public static bool IsMd5String(this string input)
        {
            return RegMd5.IsMatch(input);
        }
    }
}
