﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FclEx.Extensions
{
    public static class CryptoExtensions
    {
        public static byte[] Transform(this ICryptoTransform xfrm, byte[] plain) => xfrm.TransformFinalBlock(plain, 0, plain.Length);

        public static string TransformToBase64(this ICryptoTransform xfrm, byte[] plain) => xfrm.Transform(plain).ToBase64String();

        public static string TransformToBase64(this ICryptoTransform xfrm, string plain) => xfrm.TransformToBase64(plain.ToUtf8Bytes());
    }
}
