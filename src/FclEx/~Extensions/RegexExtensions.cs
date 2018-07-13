using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FclEx
{
    public static class RegexExtensions
    {
        public static string TryGet(this Match m, int index = 0, string defaultValue = default)
        {
            return m.Success && index >= 0 && index < m.Groups.Count
                ? m.Groups[index].Value
                : defaultValue;
        }

        public static int TryGetInt(this Match m, int index = 0, int defaultValue = default)
        {
            var s = TryGet(m, index);
            return s != null && int.TryParse(s, out var i)
                ? i
                : defaultValue;
        }

    }
}
