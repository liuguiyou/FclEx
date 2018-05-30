using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FclEx.Helpers
{
    public static class ActionHelper
    {
        internal static Action<Exception> EmptyExpAction { get; } = e => { };

        public static void Try(Action action, int retryTimes = 3, int delaySeconds = 0,
            Action<AggregateException> onFail = null, bool throwOnFail = false)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            onFail = onFail ?? EmptyExpAction;
            var times = Math.Max(0, retryTimes) + 1;
            var exList = new List<Exception>();
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception ex)
                {
                    exList.Add(ex);
                    ThreadHelper.Sleep(delaySeconds);
                }
            }

            var e = new AggregateException(exList);
            onFail(e);

            if (throwOnFail) throw e;
        }

        public static T Try<T>(Func<T> action, int retryTimes = 3, int delaySeconds = 0,
            Action<AggregateException> onFail = null, bool throwOnFail = false)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            onFail = onFail ?? EmptyExpAction;
            var times = Math.Max(0, retryTimes) + 1;
            var exList = new List<Exception>();
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    exList.Add(ex);
                    ThreadHelper.Sleep(delaySeconds);
                }
            }

            var e = new AggregateException(exList);
            onFail(e);

            if (throwOnFail) throw e;

            return default;
        }

        public static async Task TryAsync(Func<Task> func, int retryTimes = 3, int delaySeconds = 0,
            Action<AggregateException> onFail = null, bool throwOnFail = false)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            onFail = onFail ?? EmptyExpAction;
            var times = Math.Max(0, retryTimes) + 1;
            var exList = new List<Exception>();
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    await func().DonotCapture();
                    return;
                }
                catch (Exception ex)
                {
                    exList.Add(ex);
                    await TaskHelper.Delay(delaySeconds);
                }
            }

            var e = new AggregateException(exList);
            onFail(e);

            if (throwOnFail) throw e;
        }

        public static async Task<T> TryAsync<T>(Func<Task<T>> func, int retryTimes = 3, int delaySeconds = 0,
            Action<AggregateException> onFail = null, bool throwOnFail = false)
        {
            if (func == null) throw new ArgumentNullException(nameof(func));

            onFail = onFail ?? EmptyExpAction;
            var times = Math.Max(0, retryTimes) + 1;
            var exList = new List<Exception>();
            for (var i = 1; i <= times; i++)
            {
                try
                {
                    return await func().DonotCapture();
                }
                catch (Exception ex)
                {
                    exList.Add(ex);
                    await TaskHelper.Delay(delaySeconds);
                }
            }

            var e = new AggregateException(exList);
            onFail(e);

            if (throwOnFail) throw e;

            return default;
        }
    }
}
