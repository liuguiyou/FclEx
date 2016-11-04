using System.Security.Cryptography;

namespace FxUtility.Extensions
{
    public static class Md5Extensions
    {
        public static string ToMd5String(this byte[] input)
        {
            return input.ToMd5().ToHexString();
        }

        public static byte[] ToMd5(this byte[] input)
        {
            return MD5.Create().ComputeHash(input);
        }
    }
}
