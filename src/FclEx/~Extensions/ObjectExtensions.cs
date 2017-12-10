namespace FclEx
{
    public static class ObjectExtensions
    {
        public static bool IsDefault<T>(this T obj) => obj.Equals(default(T));

        public static bool IsNull<T>(this T obj) => obj == null;

        public static bool IsNotNull<T>(this T obj) => obj != null;

        public static bool IsNullOrDefault<T>(this T obj) => obj == null || obj.Equals(default(T));

        public static T CastTo<T>(this object obj) => (T)obj;

        public static string SafeToString<T>(this T obj) => obj == null ? string.Empty : obj.ToString();
    }
}
