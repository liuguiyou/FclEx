using System;

namespace FclEx.Consumers
{
    public struct ConsumerExceptionEventArgs<T>
    {
        public ConsumerExceptionEventArgs(T item, Exception exception)
        {
            Item = item;
            Exception = exception;
        }

        public T Item { get; }
        public Exception Exception { get; }
    }
}