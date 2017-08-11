using System;
using System.Collections.Generic;
using System.Text;

namespace FclEx.Models
{
    public struct DataResult<T>
    {
        public DataResult(bool successful, T data)
        {
            Successful = successful;
            Data = data;
        }

        public bool Successful { get; }
        public T Data { get; }

        public static implicit operator DataResult<T>((bool, T) result)
        {
            return new DataResult<T>(result.Item1, result.Item2);
        }

        public static implicit operator DataResult<T>((T, bool) result)
        {
            return new DataResult<T>(result.Item2, result.Item1);
        }

        public static implicit operator DataResult<T>(Tuple<bool, T> result)
        {
            return new DataResult<T>(result.Item1, result.Item2);
        }

        public static implicit operator DataResult<T>(Tuple<T, bool> result)
        {
            return new DataResult<T>(result.Item2, result.Item1);
        }

        public static implicit operator DataResult<T>(KeyValuePair<bool, T> result)
        {
            return new DataResult<T>(result.Key, result.Value);
        }

        public static implicit operator DataResult<T>(KeyValuePair<T, bool> result)
        {
            return new DataResult<T>(result.Value, result.Key);
        }
    }
}
