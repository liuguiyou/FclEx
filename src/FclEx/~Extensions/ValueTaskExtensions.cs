﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FclEx.Utils;

namespace FclEx
{
    public static class ValueTaskExtensions
    {
        public static void Forget<T>(this ValueTask<T> task) { }

        public static Task<T[]> WhenAll<T>(this IEnumerable<ValueTask<T>> tasks)
        {
            return Task.WhenAll(tasks.Select(t => t.AsTask()));
        }

        public static ValueTask<T> ToValueTask<T>(this T obj) => new ValueTask<T>(obj);

        public static ConfiguredValueTaskAwaitable<T> DonotCapture<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(false);
        }

        public static ConfiguredValueTaskAwaitable DonotCapture(this ValueTask task)
        {
            return task.ConfigureAwait(false);
        }

        public static void RunWithoutSyncContext(this ValueTask task)
        {
            NoSyncContextScope.Run(task);
        }

        public static T RunWithoutSyncContext<T>(this ValueTask<T> task)
        {
            return NoSyncContextScope.Run(task);
        }
    }
}
