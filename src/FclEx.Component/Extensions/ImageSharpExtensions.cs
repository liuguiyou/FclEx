using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImageSharp;

namespace FclEx.Extensions
{
    public static class ImageSharpExtensions
    {
        public static string ToRawBase64String(this Image<Rgba32> bitmap)
        {
            using (var m = new MemoryStream())
            {
                bitmap.Save(m);
                return m.ToArray().ToBase64String();
            }
        }

        public static Image<Rgba32> Base64StringToImage(this string base64String)
        {
            using (var m = new MemoryStream(Convert.FromBase64String(base64String)))
            {
                return ImageSharp.Image.Load(m);
            }
        }
    }
}
