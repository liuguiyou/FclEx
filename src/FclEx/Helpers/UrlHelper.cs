﻿using System;

namespace FclEx.Helpers
{
    public class UrlHelper
    {
        public static string GetOrigin(string url)
        {
            return url.Substring(0, url.LastIndexOf("/", StringComparison.Ordinal));
        }
    }
}
