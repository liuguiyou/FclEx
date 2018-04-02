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
            return type.IsValueType 
                ? (T) Convert.ChangeType(obj, type)
                : (T) obj;
        }

        public static string SafeToString<T>(this T obj) => obj == null ? string.Empty : obj.ToString();
    }
}
