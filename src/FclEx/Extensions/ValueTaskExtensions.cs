using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FclEx.Extensions
{
    public static class ValueTaskExtensions
    {
        public static void Forget<T>(this ValueTask<T> task) { }

        public static Task<T[]> WhenAll<T>(this IEnumerable<ValueTask<T>> tasks)
        {
            return Task.WhenAll(tasks.Select(t => t.AsTask()));
        }
    }
}
