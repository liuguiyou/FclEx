using System;

namespace FclEx.Consumers
{
    public static class ConsumerExArgs
    {
        public static ConsumerExArgs<T> Create<T>(T item, Exception exception)
        {
            return new ConsumerExArgs<T>(item, exception);
        }
    }

    public struct ConsumerExArgs<T>
    {
        public ConsumerExArgs(T item, Exception exception)
        {
            Item = item;
            Exception = exception;
        }

        public T Item { get; }
        public Exception Exception { get; }
    }
}