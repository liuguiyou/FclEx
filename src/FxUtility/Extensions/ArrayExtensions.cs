using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FxUtility.Extensions
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] items, T item)
        {
            if (items != null)
            {
                for (var i = 0; i < items.Length; ++i)
                {
                    if (ReferenceEquals(item, items[i])) return i;
                }
            }
            return -1;
        }

        public static int LastIndexOf<T>(this T[] items, T item)
        {
            if (items != null)
            {
                for (var i = items.Length - 1; i >= 0; --i)
                {
                    if (ReferenceEquals(item, items[i])) return i;
                }
            }
            return -1;
        }
    }
}
