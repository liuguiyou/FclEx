﻿using System;
using System.Threading.Tasks;

namespace FclEx.Consumers
{
    public class AutoRetryConsumer<T> : AbstractConsumer<AutoRetryConsumer<T>, T>
    {
        private readonly int _maxRetryTimes;
        private readonly Func<int, int> _retryDelay;

        public event EventHandler<AutoRetryConsumer<T>, ConsumerExceptionEventArgs<T>> OnException = (sender, args) => { };
        public event AsyncEventHandler<AutoRetryConsumer<T>, T> OnConsume = (sender, e) => Task.CompletedTask;
        public event AsyncEventHandler<AutoRetryConsumer<T>, T> OnDiscard = (sender, e) => Task.CompletedTask;

        public AutoRetryConsumer(int maxRetryTimes, Func<int, int> retryDelay)
        {
            if (maxRetryTimes < 0) throw new ArgumentOutOfRangeException(nameof(maxRetryTimes));
            _maxRetryTimes = maxRetryTimes;
            _retryDelay = retryDelay ?? (x => x);

            OnConsumeInternal += async (sender, item) =>
            {
                var delay = _retryDelay(item.ErrorTimes);
                if (delay > 0)
                    await Task.Delay(delay * 1000);
                await OnConsume(sender, item.Item).DonotCapture(); ;
            };

            OnExceptionInternal += (sender, args) =>
            {
                var item = args.Item;
                OnException(sender, new ConsumerExceptionEventArgs<T>(item.Item, args.Exception));

                // 以下是失败后的补救措施
                if (item.ErrorTimes++ < _maxRetryTimes)
                {
                    _items.TryAdd(item);
                    return Task.CompletedTask;
                }
                else
                {
                    return OnDiscard(sender, item.Item);
                }
            };
        }

        public AutoRetryConsumer() : this(10, null)
        {
        }
    }
}