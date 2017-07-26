﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FclEx.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// suppress warning 4014 and do not have to use "await" for a async method
        /// </summary>
        /// <param name="task"></param>
        public static void Forget(this Task task) { }

        public static Task WhenAll(this IEnumerable<Task> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return Task.WhenAll(tasks);
        }

        public static void Forget(this ConfiguredTaskAwaitable awaitable) { }

        public static void Forget<T>(this ConfiguredTaskAwaitable<T> awaitable) { }
    }
}
