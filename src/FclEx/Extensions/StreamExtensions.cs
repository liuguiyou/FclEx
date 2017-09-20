using System.IO;
using System.Text;

namespace FclEx.Extensions
{
    public static class StreamExtensions
    {
        public static string ToString(this Stream stream, Encoding encoding = null)
        {
            using (var sr = new StreamReader(stream, encoding ?? Encoding.UTF8))
            {
                return sr.ReadToEnd();
            }
        }

        public static byte[] ToBytes(this Stream stream)
        {
            var bytes = new byte[stream.Length - stream.Position];
            stream.Read(bytes, (int)stream.Position, bytes.Length);
            return bytes;
        }

        public static Stream SeekToBegin(this Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        public static MemoryStream ToStream(this byte[] bytes)
        {
            return new MemoryStream(bytes);
        }
    }
}
