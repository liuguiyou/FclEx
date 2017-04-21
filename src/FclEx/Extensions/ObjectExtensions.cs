namespace FclEx.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetOrDefault<T>(this T obj, T defaultValue) => obj == null ? defaultValue : obj;

        public static bool IsDefault<T>(this T obj) => obj.Equals(default(T));

        public static bool IsNull(this object obj) => obj == null;

        public static bool IsNotNull(this object obj) => obj != null;

        public static bool IsNullOrDefault<T>(this T? obj) where T : struct => obj == null || obj.Value.Equals(default(T));

        public static string SafeToString(this object obj) => obj?.ToString() ?? string.Empty;

        public static string ToUpperString(this object obj) => obj.ToString().ToUpper();

        public static string ToLowerString(this object obj) => obj.ToString().ToLower();

        public static T CastTo<T>(this object obj) => (T)obj;
    }
}
