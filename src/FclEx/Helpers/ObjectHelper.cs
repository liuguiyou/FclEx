using FclEx.Extensions;

namespace FclEx.Helpers
{
    public static class ObjectHelper
    {
        public static T CreateObject<T>(params object[] args)
        {
            return (T)typeof(T).CreateObject(args);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }
    }
}
