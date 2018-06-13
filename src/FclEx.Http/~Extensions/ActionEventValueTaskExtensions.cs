using System;
using System.Threading.Tasks;
using FclEx.Http.Event;

namespace FclEx.Http
{
    public static class ActionEventValueTaskExtensions
    {
        public static async ValueTask<ActionEvent> Ok(this ValueTask<ActionEvent> @this, Action<object> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsOk) action(result.Target);
            return result;
        }

        public static async ValueTask<ActionEvent> Ok(this ValueTask<ActionEvent> @this, Func<object, Task> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsOk) await action(result.Target);
            return result;
        }

        public static async ValueTask<ActionEvent<T>> Ok<T>(this ValueTask<ActionEvent<T>> @this, Action<T> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsOk) action(result.Result);
            return result;
        }

        public static async ValueTask<ActionEvent<T>> Ok<T>(this ValueTask<ActionEvent<T>> @this, Func<T, Task> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsOk) await action(result.Result);
            return result;
        }

        public static async ValueTask<ActionEvent> Error(this ValueTask<ActionEvent> @this, Action<Exception> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsError) action(result.Exception);
            return result;
        }

        public static async ValueTask<ActionEvent> Error(this ValueTask<ActionEvent> @this, Func<Exception, Task> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsError) await action(result.Exception);
            return result;
        }

        public static async ValueTask<ActionEvent<T>> Error<T>(this ValueTask<ActionEvent<T>> @this, Action<Exception> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsError) action(result.Exception);
            return result;
        }

        public static async ValueTask<ActionEvent<T>> Error<T>(this ValueTask<ActionEvent<T>> @this, Func<Exception, Task> action)
        {
            var result = await @this.DonotCapture();
            if (result.IsError) await action(result.Exception);
            return result;
        }
    }
}
