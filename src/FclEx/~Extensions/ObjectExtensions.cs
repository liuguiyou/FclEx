using System.Collections.Generic;

namespace FclEx
{
    public static class ObjectExtensions
    {
        public static bool IsDefault<T>(this T obj) => EqualityComparer<T>.Default.Equals(obj, default);

        public static bool IsNull<T>(this T obj) => obj == null;

        public static bool IsNotNull<T>(this T obj) => obj != null;

        public static TDestination CastTo<TSource, TDestination>(this TSource obj) where TSource : class
            => (TDestination)(object)obj;

        public static string SafeToString<T>(this T obj) => obj == null ? string.Empty : obj.ToString();
    }
}
