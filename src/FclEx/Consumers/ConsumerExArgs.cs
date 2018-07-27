using System;

namespace FclEx.Consumers
{
    public static class ConsumerExArgs
    {
        public static ConsumerExArgs<T> Create<T>(T item, Exception exception, int errorTimes)
        {
            return new ConsumerExArgs<T>(item, exception, errorTimes);
        }
    }

    public struct ConsumerExArgs<T>
    {
        public ConsumerExArgs(T item, Exception exception, int errorTimes)
        {
            Item = item;
            Exception = exception;
            ErrorTimes = errorTimes;
        }

        public T Item { get; }
        public int ErrorTimes { get; }
        public Exception Exception { get; }
    }
}