using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;

namespace FclEx
{
    public static class EnumerableExtensions
    {
        public static Task<TResult[]> ForEachAsync<T, TResult>(this IEnumerable<T> sequence, Func<T, Task<TResult>> action)
        {
            return Task.WhenAll(sequence.Select(action).ToArray());
        }

        public static Task ForEachAsync<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            return Task.Run(() =>
            {
                foreach (var item in sequence)
                {
                    action(item);
                }
            });
        }
    }
}
