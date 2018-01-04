using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FclEx
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
        
        public static Task DonotCapture(this Task task)
        {
            task.ConfigureAwait(false);
            return task;
        }

        public static Task<T> DonotCapture<T>(this Task<T> task)
        {
            task.ConfigureAwait(false);
            return task;
        }

        public static Task<T> ToTask<T>(this T obj) => Task.FromResult(obj);

        public static ValueTask<T> ToValueTask<T>(this T obj) => new ValueTask<T>(obj);

        public static ValueTask<T> ToValueTask<T>(this Task<T> task) => new ValueTask<T>(task);
    }
}
