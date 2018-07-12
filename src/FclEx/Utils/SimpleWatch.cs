using System;
using System.Threading.Tasks;

namespace FclEx.Utils
{
    public static class SimpleWatch
    {
        public static TimeSpan Do(Action action)
        {
            var start = DateTime.UtcNow;
            action();
            var end = DateTime.UtcNow;
            return end - start;
        }

        public static async Task<TimeSpan> DoAsync(Func<Task> action)
        {
            var start = DateTime.UtcNow;
            await action();
            var end = DateTime.UtcNow;
            return end - start;
        }

        public static async ValueTask<TimeSpan> DoAsync(Func<ValueTask> action)
        {
            var start = DateTime.UtcNow;
            await action();
            var end = DateTime.UtcNow;
            return end - start;
        }

        public static (T Ret, TimeSpan TimeSpan) Do<T>(Func<T> action)
        {
            var start = DateTime.UtcNow;
            var ret = action();
            var end = DateTime.UtcNow;
            return (ret, end - start);
        }

        public static async Task<(T Ret, TimeSpan TimeSpan)> DoAsync<T>(Func<Task<T>> action)
        {
            var start = DateTime.UtcNow;
            var ret = await action();
            var end = DateTime.UtcNow;
            return (ret, end - start);
        }

        public static async ValueTask<(T Ret, TimeSpan TimeSpan)> DoAsync<T>(Func<ValueTask<T>> action)
        {
            var start = DateTime.UtcNow;
            var ret = await action();
            var end = DateTime.UtcNow;
            return (ret, end - start);
        }
    }
}
