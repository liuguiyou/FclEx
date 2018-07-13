using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Utils;

namespace FclEx.Consumers
{
    public class BatchConsumer<T> : AbstractConsumer<BatchConsumer<T>, T>
    {
        private readonly int _batchSecondsTimeout;
        private readonly int _batchSize;
        private bool HasTimeout => _batchSecondsTimeout > 0;

        public BatchConsumer(int batchSize, int batchSecondsTimeout)
        {
            if (batchSize < 1) throw new ArgumentOutOfRangeException(nameof(batchSize));
            _batchSize = batchSize;

            if (batchSecondsTimeout < 0) throw new ArgumentOutOfRangeException(nameof(batchSecondsTimeout));
            _batchSecondsTimeout = batchSecondsTimeout;
        }

        public event EventHandler<BatchConsumer<T>, Exception> OnException = (sender, e) => { };
        public event AsyncEventHandler<BatchConsumer<T>, IReadOnlyList<T>> OnConsume = (sender, e) => Task.CompletedTask;

        private List<T> GetItems()
        {
            var startTime = DateTime.UtcNow;
            var list = new List<T>(_batchSize);
            var timeout = (HasTimeout ? 1 : 60) * 1000;
            while (list.Count < _batchSize)
            {
                try
                {
                    if (_items.TryTake(out var item, timeout, _cts.Token))
                    {
                        list.Add(item.Item);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (HasTimeout)
                {
                    var seconds = (int)Math.Ceiling((DateTime.UtcNow - startTime).TotalSeconds);
                    if (seconds >= _batchSecondsTimeout) break;
                }
            }
            return list;
        }

        protected override async Task Process()
        {
            while (!_items.IsCompleted && !_cts.IsCancellationRequested)
            {
                var items = GetItems();
                try
                {
                    if (items.IsNullOrEmpty()) continue;
                    await OnConsume(this, items).DonotCapture();
                }
                catch (Exception ex)
                {
                    OnException(this, ex);
                }
            }
            _finish.Set();
        }

        public void AddRange(ICollection<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}
