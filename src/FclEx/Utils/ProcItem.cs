using System;

namespace FclEx.Utils
{
    public static class ProcItem
    {
        public static ProcItem<T> Create<T>(T item, int errorTimes = 0)
        {
            return new ProcItem<T>(item, errorTimes);
        }

        public static ProcExItem<T> CreateEx<T>(T item, Exception exception, int errorTimes)
        {
            return new ProcExItem<T>(item, exception, errorTimes);
        }

        public static ProcExItem<T> CreateEx<T>(ProcItem<T> item, Exception exception)
        {
            return new ProcExItem<T>(item.Item, exception, item.ErrorTimes);
        }
    }

    public struct ProcItem<T>
    {
        public ProcItem(T item, int errorTimes = 0)
        {
            Item = item;
            ErrorTimes = errorTimes;
        }

        public int ErrorTimes { get; set; }
        public T Item { get; }
    }
}
