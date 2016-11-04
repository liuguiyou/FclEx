using System.Text;

namespace FxUtility.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineIf(this StringBuilder sb, string value, bool condition)
        {
            if (condition)
            {
                sb.AppendLine(value);
            }
            return sb;
        }
    }
}
