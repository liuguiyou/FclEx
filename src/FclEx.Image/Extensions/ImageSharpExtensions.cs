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
        public static Image<Rgba32> Base64StringToImage(this string base64String) => base64String.Base64StringToBytes().ToImage();

        public static Image<Rgba32> ToImage(this byte[] bytes) => ImageSharp.Image.Load(bytes);

        public static byte[] ToBytes(this Image<Rgba32> image, IImageEncoder encoder = null)
        {
            using (var m = new MemoryStream())
            {
                image.Save(m, new PngEncoder());
                return m.ToArray();
            }

        }

        public static string ToRawBase64String(this Image<Rgba32> image, IImageEncoder encoder = null) => image.ToBytes(encoder).ToBase64String();
    }
}
