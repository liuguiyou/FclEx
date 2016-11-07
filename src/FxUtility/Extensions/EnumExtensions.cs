using System;

namespace FxUtility.Extensions
{
    public static class EnumExtensions
    {

        public static int ToInt(this Enum @enum)
        {
            return Convert.ToInt32(@enum);
        }

        public static T ToEnum<T>(this string value, T defaultValue = default(T)) where T : struct, IConvertible
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            T result;
            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }
    }
}
