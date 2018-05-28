using System;
using System.Collections.Generic;
using System.Reflection;

namespace FclEx
{
    public static class ObjectExtensions
    {
        public static bool IsDefault<T>(this T obj) => EqualityComparer<T>.Default.Equals(obj, default);

        public static bool IsNull<T>(this T obj) => obj == null;

        public static bool IsNotNull<T>(this T obj) => obj != null;

        public static T CastTo<T>(this object obj)
        {
            var type = typeof(T);
            return type.IsValueType && !type.IsEnum
                ? (T)ChangeType(obj, type)
                : (T)obj;

            object ChangeType(object value, Type t)
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    if (value == null) return default;
                    t = Nullable.GetUnderlyingType(t);
                }
                return Convert.ChangeType(value, t);
            }
        }

        public static string ToStringSafely<T>(this T obj) => obj == null ? string.Empty : obj.ToString();

        public static int GetHashCodeSafely<T>(this T obj)
        {
            return obj == null ? 0 : obj.GetHashCode();
        }
    }
}
