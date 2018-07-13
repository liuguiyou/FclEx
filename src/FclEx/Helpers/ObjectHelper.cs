using System;
using System.Collections.Generic;

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

        public static void UpdateIf<T>(ref T obj, T newValue, Func<T, T, bool> condition)
        {
            if (condition(obj, newValue))
                obj = newValue;
        }

        public static void UpdateIfLessThan<T>(ref T obj, T newValue)
        {
            var cmp = Comparer<T>.Default;
            if (cmp.Compare(obj, newValue) < 0) obj = newValue;
        }

        public static void UpdateIfNotEqual<T>(ref T obj, T newValue)
        {
            var cmp = EqualityComparer<T>.Default;
            if (!cmp.Equals(obj, newValue)) obj = newValue;
        }

        public static void UpdateIfDefault<T>(ref T obj, T newValue)
        {
            var cmp = EqualityComparer<T>.Default;
            if (!cmp.Equals(obj, default)) obj = newValue;
        }

        public static void UpdateIfEmpty(ref string obj, string newValue)
        {
            if (obj.IsNullOrEmpty()) obj = newValue;
        }
    }
}
