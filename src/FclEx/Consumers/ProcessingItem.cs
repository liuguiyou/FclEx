namespace FclEx.Consumers
{
    public struct ProcessingItem<T>
    {
        public ProcessingItem(T item)
        {
            Item = item;
            ErrorTimes = 0;
        }
        public int ErrorTimes { get; set; }
        public T Item { get; }
    }
}
