using System.Text;
using FclEx.Http.Core;

namespace FclEx.Http
{
    public static class Other
    {
        public static void AppendHttpLine(this StringBuilder sb, string value)
        {
            sb.Append(value + HttpConstants.NewLine);
        }
    }
}
