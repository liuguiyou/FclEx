using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FclEx.Consumers
{
    public abstract class AbstractConsumer<TSelf, T> : IDisposable
        where TSelf : AbstractConsumer<TSelf, T>
    {
        protected CancellationTokenSource _cts;
        protected BlockingCollection<ProcessingItem<T>> _items;
        protected ManualResetEvent _finish;

        protected event AsyncEventHandler<TSelf, ConsumerExceptionEventArgs<ProcessingItem<T>>> OnExceptionInternal
            = (sender, args) => Task.CompletedTask;

        protected event AsyncEventHandler<TSelf, ProcessingItem<T>> OnConsumeInternal
            = (sender, e) => Task.CompletedTask;

        private bool TryGetItem(out ProcessingItem<T> item)
        {
            try
            {
                if (_items.TryTake(out item, 30 * 1000, _cts.Token))
                    return true;

            }
            catch (OperationCanceledException) { }

            item = default;
            return false;
        }


        protected virtual async Task Process()
        {
            while (!_items.IsCompleted && !_cts.IsCancellationRequested)
            {
                if (!TryGetItem(out var item))
                    continue;

                try
                {
                    await OnConsumeInternal((TSelf)this, item).DonotCapture();
                    item.ErrorTimes = 0;
                }
                catch (Exception ex)
                {
                    item.ErrorTimes++;
                    var args = new ConsumerExceptionEventArgs<ProcessingItem<T>>(item, ex);
                    await OnExceptionInternal((TSelf)this, args).DonotCapture(); ;
                }
            }
            _finish.Set();
        }

        public Task Start()
        {
            _cts = new CancellationTokenSource();
            _finish = new ManualResetEvent(true);
            _items = new BlockingCollection<ProcessingItem<T>>();
            return Task.Run(Process);
        }

        public virtual void Add(T item)
        {
            _items.Add(new ProcessingItem<T>(item));
        }

        public virtual void Dispose()
        {
            _cts.Cancel();
            _finish.WaitOne();
            _items?.Dispose();
        }
    }
}
