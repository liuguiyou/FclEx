using System;
using System.Text;

namespace FclEx.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIf(this StringBuilder sb, string value, bool condition)
        {
            if (condition)
                sb.AppendLine(value);
            return sb;
        }

        public static StringBuilder AppendLineIf(this StringBuilder sb, Func<string> value, bool condition)
        {
            if (condition)
                sb.AppendLine(value());
            return sb;
        }
    }
}
