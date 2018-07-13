using System;
using System.Threading.Tasks;

namespace FclEx.Utils
{
    public struct ExcuteResult
    {
        public ExcuteResult(Exception ex)
            : this(-1, ex.Message, ex.StackTrace)
        {
        }

        public ExcuteResult(int code, string msg, string stackTrace)
        {
            Code = code;
            Msg = msg;
            StackTrace = stackTrace;
        }

        public ExcuteResult(bool successful, string msg)
            : this(successful ? 0 : -1, msg, null)
        {
        }

        public bool Success => Code == 0;
        public int Code { get; }
        public string Msg { get; }
        public string StackTrace { get; }

        public static ExcuteResult SuccessResult { get; } = new ExcuteResult(true, null);

        public ExcuteResult<T> ToExplicit<T>() => new ExcuteResult<T>(Code, Msg, StackTrace);

        public static implicit operator ExcuteResult(Exception ex)
        {
            return new ExcuteResult(ex);
        }

        public static implicit operator ExcuteResult(string error)
        {
            return new ExcuteResult(-1, error, null);
        }

        public static ExcuteResult CreateError(string error)
        {
            return new ExcuteResult(false, error);
        }

        public static ExcuteResult Excute(Action action)
        {
            try
            {
                action();
                return SuccessResult;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static async Task<ExcuteResult> ExcuteAsync(Func<Task> action)
        {
            return await ExcuteAsync((Func<ValueTask>)(async () => await action().DonotCapture()))
                .DonotCapture();
        }

        public static async ValueTask<ExcuteResult> ExcuteAsync(Func<ValueTask> action)
        {
            try
            {
                await action();
                return SuccessResult;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static ExcuteResult<T> Excute<T>(Func<T> action)
        {
            try
            {
                var result = action();
                return ExcuteResult<T>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static async Task<ExcuteResult<T>> ExcuteAsync<T>(Func<Task<T>> action)
        {
            return await ExcuteAsync((Func<ValueTask<T>>)(async () => await action().DonotCapture()))
                .DonotCapture();
        }

        public static async ValueTask<ExcuteResult<T>> ExcuteAsync<T>(Func<ValueTask<T>> action)
        {
            try
            {
                var result = await action();
                return ExcuteResult<T>.CreateSuccess(result);
            }
            catch (Exception ex)
            {
                return ex;
            }
        }
    }

    public struct ExcuteResult<T>
    {
        public bool Success => Code == 0;
        public int Code { get; }
        public string Msg { get; }
        public string StackTrace { get; private set; }
        public T Result { get; }

        public ExcuteResult(Exception ex)
            : this(-1, ex.Message, ex.StackTrace)
        {
        }

        public ExcuteResult(int code, string msg, string stackTrace)
        {
            Code = code;
            Msg = msg;
            StackTrace = stackTrace;
            Result = default;
        }

        public ExcuteResult(int code, string msg, T result)
        {
            Code = code;
            Msg = msg;
            StackTrace = null;
            Result = result;
        }

        public ExcuteResult(bool successful, string msg, T result)
            : this(successful ? 0 : -1, msg, result)
        {
        }

        public static implicit operator ExcuteResult(ExcuteResult<T> result)
        {
            return new ExcuteResult(result.Code, result.Msg, result.StackTrace);
        }

        public static implicit operator ExcuteResult<T>(T item)
        {
            return item == null
                ? new ExcuteResult<T>(-1, "结果为空", default(T))
                : new ExcuteResult<T>(0, null, item);
        }

        public static implicit operator ExcuteResult<T>(Exception ex)
        {
            return new ExcuteResult<T>(ex);
        }

        public static implicit operator ExcuteResult<T>(string error)
        {
            return new ExcuteResult<T>(-1, error, default(T));
        }

        public static ExcuteResult<T> CreateSuccess(T item)
        {
            return new ExcuteResult<T>(true, null, item);
        }

        public static ExcuteResult<T> CreateError(string error)
        {
            return new ExcuteResult<T>(false, error, default);
        }
    }
}
