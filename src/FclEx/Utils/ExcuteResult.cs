using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FclEx.Utils
{
    public struct ExcuteResult
    {
        public ExcuteResult(Exception ex) : this(-1, ex)
        {
        }

        public ExcuteResult(int code, Exception ex)
        {
            Code = code;
            Exception = code == 0 ? null : ex;
        }

        public ExcuteResult(int code, string msg, string stackTrace)
            : this(code, msg == null ? null : new SimpleException(msg, stackTrace))
        {
        }

        public ExcuteResult(bool successful, string msg)
            : this(successful ? 0 : -1, new SimpleException(msg))
        {
        }

        public bool Success => Code == 0;

        public int Code { get; }
        
        public Exception Exception { get; }

        public static ExcuteResult SuccessResult { get; } = new ExcuteResult(true, null);

        public ExcuteResult<T> ToExplicit<T>() => new ExcuteResult<T>(Code, Exception);

        public static implicit operator ExcuteResult(Exception ex)
        {
            return new ExcuteResult(-1, ex);
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
            try
            {
                await action().DonotCapture();
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
            try
            {
                var result = await action().DonotCapture();
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
        public Exception Exception { get; }
        public T Result { get; }

        public ExcuteResult(Exception ex) : this(-1, ex)
        {
        }

        public ExcuteResult(int code, Exception ex)
        {
            Code = code;
            Exception = code == 0 ? null : ex;
            Result = default;
        }

        public ExcuteResult(T result)
        {
            Result = result;
            Code = 0;
            Exception = null;
        }

        public static implicit operator ExcuteResult(ExcuteResult<T> result)
        {
            return new ExcuteResult(result.Code, result.Exception);
        }

        public static implicit operator ExcuteResult<T>(T item)
        {
            return item == null
                ? new ExcuteResult<T>(-1, new SimpleException("结果为空"))
                : new ExcuteResult<T>(item);
        }

        public static implicit operator ExcuteResult<T>(Exception ex)
        {
            return new ExcuteResult<T>(ex);
        }

        public static implicit operator ExcuteResult<T>(string error)
        {
            return CreateError(error);
        }

        public static ExcuteResult<T> CreateSuccess(T item)
        {
            return new ExcuteResult<T>(item);
        }

        public static ExcuteResult<T> CreateError(string error)
        {
            return new ExcuteResult<T>(-1, new SimpleException(error));
        }
    }
}
