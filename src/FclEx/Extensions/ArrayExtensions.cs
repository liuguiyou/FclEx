using System;
using System.Collections.Generic;

namespace FclEx.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] items, T item)
        {
            return items != null ? Array.IndexOf(items, item) : -1;
        }

        public static int LastIndexOf<T>(this T[] items, T item)
        {
            return items != null ? Array.LastIndexOf(items, item) : -1;
        }

        public static void Clear<T>(this T[] items)
        {
            if (items != null)
            {
                Array.Clear(items, 0, items.Length);
            }
        }

        public static T GetAtOrDefault<T>(this IList<T> list, int index, T defaultValue = default(T))
        {
            return list != null && list.Count > index ? list[index] : defaultValue;
        }
    }
}
