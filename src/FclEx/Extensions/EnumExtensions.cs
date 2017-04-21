using System;

namespace FclEx.Extensions
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum @enum) => Convert.ToInt32(@enum);

        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            return ToEnum<T>(value, s => throw new ArgumentOutOfRangeException(nameof(value)));
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct, IConvertible
        {
            return ToEnum(value, s => defaultValue);
        }

        public static T ToEnum<T>(this string value, Func<string, T> defaultValueFunc) where T : struct, IConvertible
        {
            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValueFunc(value);
        }
    }
}
