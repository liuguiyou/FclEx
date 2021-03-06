﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using FclEx.Utils;

namespace FclEx.Consumers
{
    public class CommonConsumer<T> : AbstractConsumer<CommonConsumer<T>, T>
    {
        public event EventHandler<CommonConsumer<T>, ProcExItem<T>> OnException = (sender, args) => { };
        public event AsyncEventHandler<CommonConsumer<T>, T> OnConsume = (sender, e) => Task.CompletedTask;

        public CommonConsumer()
        {
            OnConsumeInternal += (sender, item) => OnConsume(sender, item.Item);
            OnExceptionInternal += (sender, args) =>
            {
                OnException.Invoke(sender, new ProcExItem<T>(args.Item.Item, args.Exception, args.ErrorTimes));
                return Task.CompletedTask;
            };
        }
    }
}
