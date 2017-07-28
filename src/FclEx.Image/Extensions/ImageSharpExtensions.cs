using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageSharp;
using ImageSharp.Formats;

namespace FclEx.Extensions
{
    public static class ImageSharpExtensions
    {
        public static string ToRawBase64String(this Image<Rgba32> image, IImageFormat format = null)
        {
            using (var m = new MemoryStream())
            {
                image.Save(m, format ?? image.CurrentImageFormat);
                return m.ToArray().ToBase64String();
            }
        }

        public static Image<Rgba32> Base64StringToImage(this string base64String)
        {
            return base64String.Base64StringToBytes().ToImage();
        }

        public static Image<Rgba32> ToImage(this byte[] bytes) => ImageSharp.Image.Load(bytes);

        public static byte[] ToBytes(this Image<Rgba32> image, IImageFormat format = null)
        {
            using (var m = new MemoryStream())
            {
                image.Save(m, format ?? image.CurrentImageFormat);
                return m.ToArray();
            }

        }
    }
}
