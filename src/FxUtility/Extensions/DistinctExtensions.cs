using System;
using System.Collections.Generic;
using System.Linq;
using FxUtility.Helpers;

namespace FxUtility.Extensions
{
    public static class DistinctExtensions
    {
        public static IEnumerable<T> Distinct<T, V>(this IEnumerable<T> source, Func<T, V> keySelector, IEqualityComparer<V> comparer = null)
        {
            return source.Distinct(EqualityHelper<T>.CreateComparer(keySelector, comparer));
        }
    }
}
